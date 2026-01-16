using Model;

namespace Service.Interface
{
    public interface IChatService
    {
        Task SendMessageAsync(string message);
        Task SendImageAsync(string filePath);

        event Action<ChatMessage> MessageReceived;
    }
}
