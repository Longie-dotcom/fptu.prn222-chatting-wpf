using Model;

namespace Network.Interface.Receiver
{
    public interface IChatReceiver
    {
        event Action<ChatMessage> MessageReceived;
    }
}
