using Application.Interface;
using Application.Service;
using Hub.Model;
using Hub.Runtime;
using Infrastructure.ExternalService.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

var users = new List<UserSession>();
var usersLock = new object();

var messages = new List<ChatMessage>();
var messagesLock = new object();

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Services
// -----------------------------
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IImageStorage, LocalImageStorage>();
builder.Services.AddCors();
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -----------------------------
// Swagger
// -----------------------------

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hub API V1");
        c.RoutePrefix = "swagger"; // Swagger UI at http://localhost:5000/swagger
    });


// -----------------------------
// Storage Configuration
// -----------------------------
var rootPath = builder.Configuration["Storage:RootPath"]!;
if (!Directory.Exists(rootPath))
{
    Directory.CreateDirectory(rootPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(rootPath),
    RequestPath = "/images"
});

// -----------------------------
// WebSocket & CORS
// -----------------------------
app.UseWebSockets();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// -----------------------------
// Map Controllers (ImageController)
// -----------------------------
app.MapControllers();

// -----------------------------
// Map WebSocket endpoint
// -----------------------------
app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        return;
    }

    var socket = await context.WebSockets.AcceptWebSocketAsync();
    var name = context.Request.Query["name"].ToString();
    var user = new UserSession(socket, name);

    lock (usersLock)
        users.Add(user);

    Console.WriteLine($"[CONNECT] {user.Name} ({user.UserID})");

    await SendAsync(socket, new ChatMessage
    {
        ChatMessageID = Guid.NewGuid(),
        SenderID = user.UserID,
        SenderName = user.Name,
        Type = MessageType.Handshake,
        Time = DateTime.UtcNow
    });

    ChatMessage[] history;
    lock (messagesLock)
        history = messages.ToArray();

    foreach (var msg in history)
        await SendAsync(socket, msg);

    var connected = new ChatMessage
    {
        ChatMessageID = Guid.NewGuid(),
        SenderID = user.UserID,
        SenderName = user.Name,
        Type = MessageType.Connected,
        Time = DateTime.UtcNow
    };

    StoreMessage(connected);
    await BroadcastAsync(connected);

    await ReceiveLoop(user);
});

app.Run();


// =======================================================
// RECEIVE LOOP
// =======================================================
async Task ReceiveLoop(UserSession user)
{
    var buffer = new byte[4096];

    try
    {
        while (user.Socket.State == WebSocketState.Open)
        {
            using var ms = new MemoryStream();
            WebSocketReceiveResult result;

            do
            {
                result = await user.Socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    return;

                ms.Write(buffer, 0, result.Count);

            } while (!result.EndOfMessage);

            var json = Encoding.UTF8.GetString(ms.ToArray());

            if (JsonSerializer.Deserialize<ChatMessage>(json) is not ChatMessage message)
                continue;

            // SERVER AUTHORITATIVE
            message.ChatMessageID = Guid.NewGuid();
            message.SenderID = user.UserID;
            message.SenderName = user.Name;
            message.Time = DateTime.UtcNow;

            StoreMessage(message);
            await BroadcastAsync(message);
        }
    }
    finally
    {
        lock (usersLock)
            users.Remove(user);

        Console.WriteLine($"[DISCONNECT] {user.Name} ({user.UserID})");

        if (user.Socket.State == WebSocketState.CloseReceived)
        {
            await user.Socket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closed",
                CancellationToken.None);
        }
    }
}

// =======================================================
// HELPERS
// =======================================================
void StoreMessage(ChatMessage message)
{
    lock (messagesLock)
    {
        messages.Add(message);
        if (messages.Count > 200)
            messages.RemoveAt(0);
    }
}

async Task BroadcastAsync(ChatMessage message)
{
    var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

    UserSession[] snapshot;
    lock (usersLock)
        snapshot = users.ToArray();

    foreach (var user in snapshot)
    {
        if (user.Socket.State != WebSocketState.Open)
            continue;

        try
        {
            await user.Socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        catch
        {
            lock (usersLock)
                users.Remove(user);
        }
    }
}

async Task SendAsync(WebSocket socket, ChatMessage message)
{
    var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
    await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
}
