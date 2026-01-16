using Model;

namespace WPF.View.Chat
{
    public interface IChat
    {
        event Action ViewLoaded;
        event Action<string> SendClicked;
        event Action<string> SendImageClicked;

        void ReceiveMessage(ChatMessage message);
    }
}
