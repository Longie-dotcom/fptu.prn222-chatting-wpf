using Model;

namespace Service.Interface
{
    public interface IChatService
    {
        Task SendMessageAsync(string message);

        event Action<ChatMessage> MessageReceived;
    }
}
