using Model;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Network
{
    public class Connection
    {
        private readonly ClientWebSocket ws = new();
        private readonly CancellationTokenSource cts = new();

        public event Action<ChatMessage>? MessageReceived;

        public Connection(string serverUri)
        {
            _ = ConnectAndListenAsync(serverUri);
        }

        public async Task SendMessageAsync(ChatMessage message)
        {
            if (ws.State != WebSocketState.Open)
                return;

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            await ws.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                cts.Token);
        }

        private async Task ConnectAndListenAsync(string uri)
        {
            await ws.ConnectAsync(new Uri(uri), cts.Token);
            await ReceiveLoopAsync();
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[4096];

            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cts.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var data = JsonSerializer.Deserialize<ChatMessage>(json);

                if (data != null)
                    MessageReceived?.Invoke(data);
            }
        }
    }
}
