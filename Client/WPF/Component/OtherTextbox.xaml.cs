using System.Windows.Controls;

namespace WPF.Component
{
    public partial class OtherTextbox : UserControl
    {
        public OtherTextbox()
        {
            InitializeComponent();
        }

        public string NameText
        {
            get => UserName.Text;
            set => UserName.Text = value;
        }

        public string MessageText
        {
            get => ChatMessage.GetText();
            set => ChatMessage.SetTextWithEmojis(value);
        }
    }
}
