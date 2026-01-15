using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF.Component
{
    public partial class SpriteButton : UserControl
    {
        public SpriteButton()
        {
            InitializeComponent();

            // Initial background
            Loaded += (s, e) => SetBackground(NormalPath);

            // Mouse events
            Button.MouseEnter += (s, e) => SetBackground(HoverPath ?? NormalPath);
            Button.MouseLeave += (s, e) => SetBackground(NormalPath);
            Button.PreviewMouseDown += (s, e) => SetBackground(PressedPath ?? NormalPath);
            Button.PreviewMouseUp += (s, e) => SetBackground(HoverPath ?? NormalPath);

            // Click forwarding
            Button.Click += (s, e) => Click?.Invoke(s, e);
        }

        public event RoutedEventHandler Click;

        public string NormalPath
        {
            get => (string)GetValue(NormalPathProperty);
            set => SetValue(NormalPathProperty, value);
        }
        public static readonly DependencyProperty NormalPathProperty =
            DependencyProperty.Register(nameof(NormalPath), typeof(string), typeof(SpriteButton));

        public string HoverPath
        {
            get => (string)GetValue(HoverPathProperty);
            set => SetValue(HoverPathProperty, value);
        }
        public static readonly DependencyProperty HoverPathProperty =
            DependencyProperty.Register(nameof(HoverPath), typeof(string), typeof(SpriteButton));

        public string PressedPath
        {
            get => (string)GetValue(PressedPathProperty);
            set => SetValue(PressedPathProperty, value);
        }
        public static readonly DependencyProperty PressedPathProperty =
            DependencyProperty.Register(nameof(PressedPath), typeof(string), typeof(SpriteButton));

        public double ButtonWidth
        {
            get => (double)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }
        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register(nameof(ButtonWidth), typeof(double), typeof(SpriteButton), new PropertyMetadata(64.0));

        public double ButtonHeight
        {
            get => (double)GetValue(ButtonHeightProperty);
            set => SetValue(ButtonHeightProperty, value);
        }
        public static readonly DependencyProperty ButtonHeightProperty =
            DependencyProperty.Register(nameof(ButtonHeight), typeof(double), typeof(SpriteButton), new PropertyMetadata(64.0));

        private void SetBackground(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            string fullPath = Path.Combine(AppContext.BaseDirectory, path.Replace("/", "\\"));
            if (!File.Exists(fullPath))
            {
                System.Diagnostics.Debug.WriteLine($"SpriteButton: File not found -> {fullPath}");
                return;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // load immediately
                bitmap.EndInit();

                Button.Background = new ImageBrush(bitmap)
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Center,
                    AlignmentY = AlignmentY.Center
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SpriteButton: Failed to load image {fullPath} -> {ex.Message}");
            }
        }
    }
}
