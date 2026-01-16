using Model;

namespace Network.Interface.Command
{
    public interface IChatCommand
    {
        Task SendMessageAsync(string message, Guid senderId, MessageType type);
        Task SendImageAsync(string filePath, Guid senderId);
    }
}
