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

            var window = new View.Lobby.Lobby();
            window.Show();
        }
    }
}
