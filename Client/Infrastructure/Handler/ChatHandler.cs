using Model;
using Network.Interface.Command;
using Network.Interface.Receiver;

namespace Network.Handler
{
    public class ChatHandler :
        IChatCommand,
        IChatReceiver
    {
        private readonly Connection connection;

        public event Action<ChatMessage>? MessageReceived;

        public ChatHandler(Connection connection)
        {
            this.connection = connection;

            connection.MessageReceived += msg =>
            {
                MessageReceived?.Invoke(msg);
            };
        }

        public async Task SendMessageAsync(string message, Guid senderId)
        {
            // Map DTO
            var chatMessage = new ChatMessage()
            {
                Message = message,
                SenderID = senderId,
                ChatMessageID = Guid.NewGuid(),
                Time = DateTime.Now,
                Type = MessageType.Message,
            };

            await connection.SendMessageAsync(chatMessage);
        }
    }
}
