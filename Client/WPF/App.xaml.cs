using Network;
using Network.Handler;
using Service.Implementation;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using WPF.Presenter;

namespace WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            base.OnStartup(e);

            //// Infrastructure
            //var connection = new Connection(
            //    "ws://26.92.115.30:5000/ws?name=BaoLong");

            //// Network
            //var handler = new ChatHandler(connection);

            //// Application
            //var chatService = new ChatService(handler, handler);

            // View
            var window = new View.Lobby.Lobby();

            // Presenter(binds View + Service)
            //var presenter = new ChatPresenter(window, chatService);

            window.Show();
        }
    }
}
