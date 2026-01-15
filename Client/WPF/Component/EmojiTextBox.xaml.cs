using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF.Component
{
    public partial class EmojiTextBox : UserControl
    {
        // --- Emoji map ---
        private readonly Dictionary<string, string> emojiMap = new Dictionary<string, string>
        {
            { ":iron-sword:", "pack://application:,,,/Asset/Component/Emoji/iron-sword.png" },
            { ":wooden-sword:", "pack://application:,,,/Asset/Component/Emoji/wooden-sword.png" },
            { ":mushroom:", "pack://application:,,,/Asset/Component/Emoji/mushroom.png" },
            { ":stone:", "pack://application:,,,/Asset/Component/Emoji/stone.png" },
            { ":wood:", "pack://application:,,,/Asset/Component/Emoji/wood.png" },
        };

        private bool _isUpdating = false;
        private string _textWithEmojiCodes = "";

        public event KeyEventHandler OnEnterPressed;

        public EmojiTextBox()
        {
            InitializeComponent();

            // Bind RichTextBox properties
            rtbInput.SetBinding(RichTextBox.FontFamilyProperty, new System.Windows.Data.Binding("FontFamily") { Source = this });
            rtbInput.SetBinding(RichTextBox.FontSizeProperty, new System.Windows.Data.Binding("FontSize") { Source = this });
            rtbInput.SetBinding(RichTextBox.ForegroundProperty, new System.Windows.Data.Binding("ForeColor") { Source = this });

            rtbInput.PreviewKeyDown += RtbInput_PreviewKeyDown;
            rtbInput.TextChanged += RtbInput_TextChanged;

            // Single-line setup
            rtbInput.AcceptsReturn = false;
            rtbInput.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            rtbInput.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        // --- Exposed properties ---
        public new double Width { get => base.Width; set => base.Width = value; }
        public new double Height { get => base.Height; set => base.Height = value; }

        public Brush ForeColor
        {
            get => (Brush)GetValue(ForeColorProperty);
            set => SetValue(ForeColorProperty, value);
        }
        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Brush), typeof(EmojiTextBox), new PropertyMetadata(Brushes.Black));

        public new FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        public static new readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(EmojiTextBox), new PropertyMetadata(new FontFamily("Segoe UI")));

        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public static new readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(EmojiTextBox), new PropertyMetadata(14.0));

        public bool IsReadOnly
        {
            get => rtbInput.IsReadOnly;
            set
            {
                rtbInput.IsReadOnly = value;
                rtbInput.Cursor = value ? Cursors.Arrow : Cursors.IBeam;
            }
        }

        public string TextWithEmojiCodes
        {
            get => _textWithEmojiCodes;
            set
            {
                _textWithEmojiCodes = value ?? "";
                SetTextWithEmojis(_textWithEmojiCodes);
            }
        }

        public bool WordWrap
        {
            get => (bool)GetValue(WordWrapProperty);
            set => SetValue(WordWrapProperty, value);
        }

        public static readonly DependencyProperty WordWrapProperty =
            DependencyProperty.Register("WordWrap", typeof(bool), typeof(EmojiTextBox),
                new PropertyMetadata(false, OnWordWrapChanged));

        public string GetRawText()
        {
            return CalculateRawText();
        }

        // --- Live input handling ---
        private void RtbInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent the newline
                OnEnterPressed?.Invoke(this, e); // Notify the Chat window
            }
        }

        private void RtbInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating || IsReadOnly) return;
            _isUpdating = true;

            // Temporarily disable events to prevent infinite loops
            rtbInput.TextChanged -= RtbInput_TextChanged;

            var caret = rtbInput.CaretPosition;
            ReplaceEmojisInline();

            // Refresh our internal raw text variable by parsing the document
            _textWithEmojiCodes = CalculateRawText();

            rtbInput.CaretPosition = caret;
            rtbInput.TextChanged += RtbInput_TextChanged;

            _isUpdating = false;
        }

        private string CalculateRawText()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (var block in rtbInput.Document.Blocks)
            {
                if (block is Paragraph para)
                {
                    foreach (var inline in para.Inlines)
                    {
                        if (inline is Run run)
                        {
                            sb.Append(run.Text);
                        }
                        else if (inline is InlineUIContainer container)
                        {
                            // This pulls the ":stone:" code back out of the Tag we stored
                            if (container.Tag != null)
                                sb.Append(container.Tag.ToString());
                        }
                    }
                }
            }
            // RichTextBox often adds a trailing \r\n at the end of the document
            return sb.ToString().TrimEnd('\r', '\n');
        }

        private void ReplaceEmojisInline()
        {
            TextPointer pointer = rtbInput.Document.ContentStart;

            while (pointer != null && pointer.CompareTo(rtbInput.Document.ContentEnd) < 0)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                    foreach (var kvp in emojiMap)
                    {
                        int idx = textRun.IndexOf(kvp.Key, StringComparison.Ordinal);
                        if (idx >= 0)
                        {
                            TextPointer start = pointer.GetPositionAtOffset(idx);
                            TextPointer end = start.GetPositionAtOffset(kvp.Key.Length);

                            // 1. Remove the text code
                            new TextRange(start, end).Text = "";

                            // 2. Create the image
                            var img = new Image
                            {
                                Source = new BitmapImage(new Uri(kvp.Value, UriKind.Absolute)),
                                Width = 32,
                                Height = 32,
                                Stretch = Stretch.Uniform
                            };

                            // 3. Create container and STORE THE KEY in the Tag
                            var container = new InlineUIContainer(img, start)
                            {
                                BaselineAlignment = BaselineAlignment.Center,
                                Tag = kvp.Key // <--- CRITICAL: Store ":stone:" here
                            };
                        }
                    }
                }
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        // --- Programmatic set for readonly chat ---
        public void SetTextWithEmojis(string text)
        {
            _isUpdating = true;
            rtbInput.Document.Blocks.Clear();

            // Use a smaller margin for the paragraph to avoid extra spacing
            var para = new Paragraph { Margin = new Thickness(0) };

            if (!WordWrap)
            {
                // SINGLE LINE MODE (Input Box)
                // We need enough height to fit the 32px emoji without clipping
                para.LineHeight = 36;
                para.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            }
            else
            {
                // MULTI LINE MODE (Chat Bubble)
                // Set MaxHeight or let it grow naturally. 
                // StackingStrategy.MaxHeight ensures the line grows to fit the tallest item (the 32px emoji)
                para.LineStackingStrategy = LineStackingStrategy.MaxHeight;
            }

            int lastIndex = 0;
            while (lastIndex < text.Length)
            {
                int nextEmojiIndex = -1;
                string nextEmojiKey = null;

                foreach (var kvp in emojiMap)
                {
                    int idx = text.IndexOf(kvp.Key, lastIndex, StringComparison.Ordinal);
                    if (idx >= 0 && (nextEmojiIndex == -1 || idx < nextEmojiIndex))
                    {
                        nextEmojiIndex = idx;
                        nextEmojiKey = kvp.Key;
                    }
                }

                if (nextEmojiIndex == -1)
                {
                    para.Inlines.Add(new Run(text.Substring(lastIndex)));
                    break;
                }

                if (nextEmojiIndex > lastIndex)
                    para.Inlines.Add(new Run(text.Substring(lastIndex, nextEmojiIndex - lastIndex)));

                // Create the Emoji Image
                var img = new Image
                {
                    Source = new BitmapImage(new Uri(emojiMap[nextEmojiKey], UriKind.Absolute)),
                    Width = 32,
                    Height = 32,
                    Stretch = Stretch.Uniform,
                    // Smooth out pixel art if necessary
                    SnapsToDevicePixels = true
                };

                // Important: Use the Tag so GetRawText still works!
                var container = new InlineUIContainer(img)
                {
                    BaselineAlignment = BaselineAlignment.Center,
                    Tag = nextEmojiKey
                };

                para.Inlines.Add(container);

                lastIndex = nextEmojiIndex + nextEmojiKey.Length;
            }

            rtbInput.Document.Blocks.Add(para);
            _isUpdating = false;
        }

        // --- Helpers ---
        private static void OnWordWrapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (EmojiTextBox)d;
            bool wrap = (bool)e.NewValue;

            if (wrap)
            {
                // For Chat History: Let the text wrap naturally
                control.rtbInput.Document.PageWidth = double.NaN;
                control.rtbInput.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                control.Height = double.NaN; // Allow height to grow
                control.MinHeight = 0;
            }
            else
            {
                // For Chat Input: Force single line
                control.rtbInput.Document.PageWidth = 10000;
                control.rtbInput.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                control.Height = 32;
            }
        }

        public string GetText()
        {
            return _textWithEmojiCodes;
        }

        public void ClearValue()
        {
            rtbInput.Document.Blocks.Clear();
            _textWithEmojiCodes = "";
        }
    }
}
