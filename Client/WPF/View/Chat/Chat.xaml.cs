using Model;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WPF.Component;
using static System.Net.Mime.MediaTypeNames;

namespace WPF.View.Chat
{
    public partial class Chat : Window, IChat
    {
        public event Action<string> SendClicked;
        public event Action ViewLoaded;

        public Chat()
        {
            InitializeComponent();
            ViewLoaded?.Invoke();

            // Inbound
            UploadImageButton.Click += UploadImageButton_Click;
            UploadFileButton.Click += UploadFileButton_Click;
            EmoteButton.Click += EmoteButton_Click;
            ChatInput.OnEnterPressed += (s, e) => HandleSend();
            RootGrid.MouseLeftButtonDown += GridRoot_MouseLeftButtonDown;
        }

        public void ReceiveMessage(ChatMessage message)
        {
            if (message.Type == MessageType.Connected)
            {
                var noti = new Notification()
                {
                    Text = $"{message.SenderName} has been connected",
                };

                ChatPanel.Children.Add(noti);
            }
            else
            {
                if (message.IsOwner)
                {
                    var msg = new PlayerTextbox
                    {
                        MessageText = message.Message,
                        NameText = message.SenderName,
                        Margin = new Thickness(0, 4, 0, 4) // spacing between messages
                    };

                    ChatPanel.Children.Add(msg);
                }
                else
                {
                    var msg = new OtherTextbox
                    {
                        MessageText = message.Message,
                        NameText = message.SenderName,
                        Margin = new Thickness(0, 4, 0, 4) // spacing between messages
                    };

                    ChatPanel.Children.Add(msg);
                }
            }

            // Scroll to bottom
            ChatScrollViewer.UpdateLayout();
            ChatScrollViewer.ScrollToBottom();
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Upload Image Clicked!");
        }
        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Upload File Clicked!");
        }
        private void EmoteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Emote Button Clicked!");
        }

        private void HandleSend()
        {
            string text = ChatInput.GetRawText().Trim();
            Debug.WriteLine($"SENDING: {text}");

            if (!string.IsNullOrEmpty(text))
            {
                SendClicked?.Invoke(text);
                ChatInput.ClearValue();
            }
        }

        private void GridRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
