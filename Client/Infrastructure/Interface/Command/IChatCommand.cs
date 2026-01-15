using Model;

namespace Network.Interface.Command
{
    public interface IChatCommand
    {
        Task SendMessageAsync(string message, Guid senderId);
    }
}
