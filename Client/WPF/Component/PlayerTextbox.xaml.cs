using Model; 
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPF.Component
{
    public partial class PlayerTextbox : UserControl
    {
        public PlayerTextbox()
        {
            InitializeComponent();
        }

        // Name of the sender
        public string NameText
        {
            get => UserName.Text;
            set => UserName.Text = value;
        }

        // For backward compatibility: just returns text content
        public string MessageText
        {
            get => ChatMessage.GetText();
            set => SetMessage(value, MessageType.Message);
        }

        /// <summary>
        /// Set message content with explicit type (text or image)
        /// </summary>
        public void SetMessage(string message, MessageType type)
        {
            if (type == MessageType.Image)
            {
                ChatMessage.Visibility = Visibility.Collapsed;
                ChatImage.Visibility = Visibility.Visible;
                Debug.WriteLine(message);

                try
                {
                    ChatImage.Source = new BitmapImage(new Uri(message));
                }
                catch
                {
                    // fallback to text if image fails
                    ChatMessage.Visibility = Visibility.Visible;
                    ChatImage.Visibility = Visibility.Collapsed;
                    ChatMessage.SetTextWithEmojis(message);
                }
            }
            else
            {
                ChatMessage.Visibility = Visibility.Visible;
                ChatImage.Visibility = Visibility.Collapsed;
                ChatMessage.SetTextWithEmojis(message);
            }
        }
    }
}
