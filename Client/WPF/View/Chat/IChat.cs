using Model;

namespace WPF.View.Chat
{
    public interface IChat
    {
        event Action ViewLoaded;
        event Action<string> SendClicked;

        void ReceiveMessage(ChatMessage message);
    }
}
