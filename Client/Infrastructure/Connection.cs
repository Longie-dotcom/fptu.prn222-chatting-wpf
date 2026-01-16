using Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Network
{
    public class Connection
    {
        private readonly ClientWebSocket _ws = new();
        private readonly HttpClient _httpClient = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly string _imageUploadApiBaseUrl;

        public event Action<ChatMessage>? MessageReceived;

        public Connection(string serverUri, string imageUploadApiBaseUrl)
        {
            _imageUploadApiBaseUrl = imageUploadApiBaseUrl;
            _ = ConnectAndListenAsync(serverUri);
        }

        /// <summary>
        /// Send a ChatMessage over WebSocket
        /// </summary>
        public async Task SendMessageAsync(ChatMessage message)
        {
            if (_ws.State != WebSocketState.Open)
                return;

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            await _ws.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                _cts.Token);
        }

        /// <summary>
        /// Upload image via HTTP and return the full URL
        /// </summary>
        public async Task<string> UploadImageAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Image file not found", filePath);

            using var form = new MultipartFormDataContent();
            using var stream = File.OpenRead(filePath);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png"); // detect dynamically if you want
            form.Add(fileContent, "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync(_imageUploadApiBaseUrl, form);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("imageName", out var prop))
                throw new InvalidOperationException("Upload API did not return imageName");

            var url = prop.GetString() ?? throw new InvalidOperationException("imageName is null");

            // Return full URL
            return new Uri(new Uri(_imageUploadApiBaseUrl).GetLeftPart(UriPartial.Authority) + "/images/" + url).ToString();
        }

        private async Task ConnectAndListenAsync(string uri)
        {
            await _ws.ConnectAsync(new Uri(uri), _cts.Token);
            await ReceiveLoopAsync();
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[4096];

            while (_ws.State == WebSocketState.Open)
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);

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
