using Model;
using Service.Interface;
using WPF.View.Chat;

namespace WPF.Presenter
{
    public class ChatPresenter
    {
        private readonly IChat chat;
        private readonly IChatService chatService;

        public ChatPresenter(
            IChat chat,
            IChatService chatService)
        {
            this.chat = chat;
            this.chatService = chatService;

            // Inbound
            chat.ViewLoaded += OnViewLoaded;
            chat.SendClicked += OnSendClicked;
            chat.SendImageClicked += OnSendImage;

            // Outbound
            chatService.MessageReceived += OnMessageReceived;
        }

        private void OnViewLoaded()
        {

        }

        private async void OnSendClicked(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            await chatService.SendMessageAsync(text);
        }

        private void OnMessageReceived(ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Message))
                return;

            chat.ReceiveMessage(message);
        }

        private async void OnSendImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            await chatService.SendImageAsync(path);
        }
    }
}
