using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WPF.Component
{
    public partial class EmojiPicker : UserControl
    {
        public event Action<string> EmojiSelected;

        private readonly Dictionary<string, string> emojiMap = new Dictionary<string, string>
        {
            { ":emoji-normal:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame1.png" },
            { ":emoji-happy:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame2.png" },
            { ":emoji-sad:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame3.png" },
            { ":emoji-disgusted:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame4.png" },
            { ":emoji-angry:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame5.png" },
            { ":emoji-cute:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame6.png" },
            { ":emoji-dumb:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame7.png" },
            { ":emoji-duh:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame8.png" },
            { ":emoji-clown", "pack://application:,,,/Asset/Component/Emoji/emoji-frame9.png" },
            { ":emoji-fish:", "pack://application:,,,/Asset/Component/Emoji/emoji-frame10.png" },
            { ":iron-sword:", "pack://application:,,,/Asset/Component/Emoji/iron-sword.png" },
            { ":wooden-sword:", "pack://application:,,,/Asset/Component/Emoji/wooden-sword.png" },
            { ":mushroom:", "pack://application:,,,/Asset/Component/Emoji/mushroom.png" },
            { ":stone:", "pack://application:,,,/Asset/Component/Emoji/stone.png" },
            { ":wood:", "pack://application:,,,/Asset/Component/Emoji/wood.png" },
        };

        public EmojiPicker()
        {
            InitializeComponent();
            LoadEmojis();
        }

        private void LoadEmojis()
        {
            EmojiWrapPanel.Children.Clear();

            foreach (var kvp in emojiMap)
            {
                var img = new Image
                {
                    Source = new BitmapImage(new Uri(kvp.Value, UriKind.Absolute)),
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(4),
                    Cursor = Cursors.Hand,
                    Tag = kvp.Key
                };

                img.MouseLeftButtonUp += (s, e) =>
                {
                    EmojiSelected?.Invoke(kvp.Key);
                };

                EmojiWrapPanel.Children.Add(img);
            }
        }
    }

}
