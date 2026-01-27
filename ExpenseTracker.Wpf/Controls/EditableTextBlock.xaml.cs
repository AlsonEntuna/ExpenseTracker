using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExpenseTracker.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for EditableTextBlock.xaml
    /// </summary>
    public partial class EditableTextBlock : UserControl
    {
        // TextSource
        public string TextSource
        {
            get => (string)GetValue(TextSourceProperty);
            set => SetValue(TextSourceProperty, value);
        }
        public static readonly DependencyProperty TextSourceProperty =
            DependencyProperty.Register(
                nameof(TextSource),
                typeof(string),
                typeof(EditableTextBlock),
                new FrameworkPropertyMetadata(
                    "",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );
        // Get
        public static string GetTextSource(UIElement target) => (string)target.GetValue(TextSourceProperty);
        // Set
        public static void SetTextSource(UIElement target, string value) => target.SetValue(TextSourceProperty, value);

        // TriggerObject
        // TODO: find a better name for this, it seems weird
        public bool TriggerObject
        {
            get => (bool)GetValue(TriggerObjectProperty);
            set => SetValue(TriggerObjectProperty, value);
        }
        public static readonly DependencyProperty TriggerObjectProperty =
            DependencyProperty.Register(
                nameof(TriggerObject),
                typeof(bool),
                typeof(EditableTextBlock),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );
        // Get
        public static bool GetTriggerObject(UIElement target) => (bool)target.GetValue(TriggerObjectProperty);
        // Set
        public static void SetTriggerObject(UIElement target, bool value) => target.SetValue(TriggerObjectProperty, value);

        private string _originalSourceText;
        public EditableTextBlock()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TriggerObject = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                TextSource = _originalSourceText ?? string.Empty;
                TriggerObject = false;
                e.Handled = false;
            }
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _originalSourceText = TextSource;
                TriggerObject = true;

                EditorTextBox.Focus();
                EditorTextBox.SelectAll();

                e.Handled = true;
            }
        }

        private void EditorTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TriggerObject = false;
        }
    }
}
