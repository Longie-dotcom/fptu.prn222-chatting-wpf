using System.Windows.Controls;

namespace WPF.Component
{
    public partial class Notification : UserControl
    {
        public Notification()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => NotificationText.Text;
            set => NotificationText.Text = value;
        }
    }
}
