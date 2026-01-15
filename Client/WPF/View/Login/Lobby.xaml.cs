using Network;
using Network.Handler;
using Service.Implementation;
using System.Windows;
using WPF.Presenter;

namespace WPF.View.Lobby
{
    public partial class Lobby : Window, ILobby
    {
        public Lobby()
        {
            InitializeComponent();

            LoginButton.Click += LoginButton_Click;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UserNameInput.Text))
                return;

            string userName = UserNameInput.Text;

            // URL-encode the username to handle spaces or special characters
            string encodedName = Uri.EscapeDataString(userName);

            // Infrastructure
            var connection = new Connection(
                $"ws://192.168.1.135:5000/ws?name={encodedName}");

            // Network
            var handler = new ChatHandler(connection);

            // Application
            var chatService = new ChatService(handler, handler);

            // View: pass username to Chat window
            var chatWindow = new View.Chat.Chat();

            // Presenter(binds View + Service)
            var presenter = new ChatPresenter(chatWindow, chatService);

            chatWindow.Show();

            // Close current Lobby window
            this.Close();
        }
    }
}