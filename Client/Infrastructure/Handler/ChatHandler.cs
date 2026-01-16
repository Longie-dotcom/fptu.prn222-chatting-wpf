using Model;
using Network.Interface.Command;
using Network.Interface.Receiver;

namespace Network.Handler
{
    public class ChatHandler : IChatCommand, IChatReceiver
    {
        private readonly Connection _connection;
        public event Action<ChatMessage>? MessageReceived;

        public ChatHandler(Connection connection)
        {
            _connection = connection;
            _connection.MessageReceived += msg => MessageReceived?.Invoke(msg);
        }

        public async Task SendMessageAsync(string message, Guid senderId, MessageType type)
        {
            var chatMessage = new ChatMessage
            {
                Message = message,
                SenderID = senderId,
                ChatMessageID = Guid.NewGuid(),
                Time = DateTime.Now,
                Type = type
            };

            await _connection.SendMessageAsync(chatMessage);
        }

        public async Task SendImageAsync(string filePath, Guid senderId)
        {
            // Upload via HTTP
            var url = await _connection.UploadImageAsync(filePath);

            // Send the image URL as a WebSocket message
            await SendMessageAsync(url, senderId, MessageType.Image);
        }
    }
}
