using Microsoft.Win32;
using Model;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using WPF.Component;
using static System.Net.Mime.MediaTypeNames;

namespace WPF.View.Chat
{
    public partial class Chat : Window, IChat
    {
        public event Action<string> SendClicked;
        public event Action<string> SendImageClicked;
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
            SendButton.Click += (s, e) => HandleSend();
            RootGrid.MouseLeftButtonDown += GridRoot_MouseLeftButtonDown;

            EmojiPickerControl.EmojiSelected += code =>
            {
                // Insert at the **caret position** instead of just appending
                var rtb = ChatInput;
                var caret = rtb.rtbInput.CaretPosition; // Access internal RichTextBox
                if (caret != null)
                {
                    rtb.TextWithEmojiCodes = rtb.TextWithEmojiCodes.Insert(rtb.GetRawText().Length, code);
                    rtb.SetTextWithEmojis(rtb.TextWithEmojiCodes);
                }

                EmojiPopup.IsOpen = false; // Close after selecting
            };
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
                UserControl msgControl;

                if (message.IsOwner)
                {
                    msgControl = new PlayerTextbox
                    {
                        NameText = message.SenderName,
                        Margin = new Thickness(0, 4, 0, 4)
                    };
                }
                else
                {
                    msgControl = new OtherTextbox
                    {
                        NameText = message.SenderName,
                        Margin = new Thickness(0, 4, 0, 4)
                    };
                }

                // Pass message content and type
                if (msgControl is PlayerTextbox playerBox)
                    playerBox.SetMessage(message.Message, message.Type);
                else if (msgControl is OtherTextbox otherBox)
                    otherBox.SetMessage(message.Message, message.Type);

                ChatPanel.Children.Add(msgControl);
            }

            ChatScrollViewer.UpdateLayout();
            ChatScrollViewer.ScrollToBottom();
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an OpenFileDialog
            var dlg = new OpenFileDialog
            {
                Title = "Select an image",
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp",
                Multiselect = false
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string filePath = dlg.FileName;
                // Raise the SendImageClicked event with the file path
                SendImageClicked?.Invoke(filePath);
            }
        }
        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Upload File Clicked!");
        }
        private void EmoteButton_Click(object sender, RoutedEventArgs e)
        {
            EmojiPopup.IsOpen = !EmojiPopup.IsOpen; // toggle popup visibility
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
