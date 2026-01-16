using Model;
using Network.Interface.Command;
using Network.Interface.Receiver;
using Service.Interface;
using System.Diagnostics;

namespace Service.Implementation
{
    public class ChatService : IChatService
    {
        private readonly IChatCommand chatCommand;

        public Guid SenderID { get; private set; }
        public event Action<ChatMessage>? MessageReceived;

        public ChatService(
            IChatCommand chatCommand,
            IChatReceiver chatReceiver)
        {
            this.chatCommand = chatCommand;
            chatReceiver.MessageReceived += msg =>
            {
                switch (msg.Type)
                {
                    case MessageType.Handshake:
                        HandleHandshake(msg);
                        break;

                    case MessageType.Connected:
                    case MessageType.Message:
                    case MessageType.Image:
                    case MessageType.File:
                        HandleChatMessage(msg);
                        break;

                    default:
                        Debug.WriteLine($"[WARN] Unknown message type: {msg.Type}");
                        break;
                }
            };
        }

        public async Task SendImageAsync(string filePath)
        {
            await chatCommand.SendImageAsync(filePath, SenderID);
        }

        public async Task SendMessageAsync(string message)
        {
            await chatCommand.SendMessageAsync(message, SenderID, MessageType.Message);
        }

        private void HandleHandshake(ChatMessage msg)
        {
            SenderID = msg.SenderID;
            MessageReceived?.Invoke(msg);
        }

        private void HandleChatMessage(ChatMessage msg)
        {
            msg.IsOwner = msg.SenderID == SenderID;
            MessageReceived?.Invoke(msg);
        }
    }
}
