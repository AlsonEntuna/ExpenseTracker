using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ExpenseTracker.WinApi;

namespace ExpenseTracker.Wpf.Controls
{
    /// <summary>
    /// Borderless window with a custom title bar, acrylic blur and Win11 rounded corners.
    /// Its look is defined by the "ExWindowStyle" style in Resources/Theme/Window.xaml.
    /// </summary>
    [TemplatePart(Name = PartTitleBar, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PartMinimizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PartMaximizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PartCloseButton, Type = typeof(Button))]
    public class ExWindow : Window
    {
        private const string PartTitleBar = "PART_TitleBar";
        private const string PartMinimizeButton = "PART_MinimizeButton";
        private const string PartMaximizeButton = "PART_MaximizeButton";
        private const string PartCloseButton = "PART_CloseButton";

        private FrameworkElement _titleBar;
        private Button _minimizeButton;
        private Button _maximizeButton;
        private Button _closeButton;

        /// <summary>
        /// Text shown in the custom title bar.
        /// </summary>
        public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(
            nameof(WindowTitle), typeof(string), typeof(ExWindow), new PropertyMetadata(string.Empty));

        public string WindowTitle
        {
            get => (string)GetValue(WindowTitleProperty);
            set => SetValue(WindowTitleProperty, value);
        }

        private IntPtr _hwnd;

        // Corner preference used when the window is not maximized.
        private WindowCornerPreference _cornerPref = WindowCornerPreference.Round;

        public ExWindow()
        {
            // Implicit type styles don't apply to subclasses (e.g. MainWindow), so reference the style by key.
            SetResourceReference(StyleProperty, "ExWindowStyle");
        }

        public override void OnApplyTemplate()
        {
            if (_titleBar != null)
                _titleBar.MouseDown -= TitleBar_MouseDown;

            if (_minimizeButton != null)
                _minimizeButton.Click -= MinimizeButton_Click;

            if (_maximizeButton != null)
                _maximizeButton.Click -= MaximizeButton_Click;

            if (_closeButton != null)
                _closeButton.Click -= CloseButton_Click;

            base.OnApplyTemplate();

            _titleBar = GetTemplateChild(PartTitleBar) as FrameworkElement;
            _minimizeButton = GetTemplateChild(PartMinimizeButton) as Button;
            _maximizeButton = GetTemplateChild(PartMaximizeButton) as Button;
            _closeButton = GetTemplateChild(PartCloseButton) as Button;

            if (_titleBar != null)
                _titleBar.MouseDown += TitleBar_MouseDown;

            if (_minimizeButton != null)
                _minimizeButton.Click += MinimizeButton_Click;

            if (_maximizeButton != null)
                _maximizeButton.Click += MaximizeButton_Click;

            if (_closeButton != null)
                _closeButton.Click += CloseButton_Click;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(_hwnd)?.AddHook(MonitorHelper.MaximizeToWorkAreaHook);

            WindowEffects.EnableMica(_hwnd);
            WindowEffects.EnableAcrylicBlur(_hwnd);
            ApplyCornerPreference();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (_hwnd == IntPtr.Zero)
                return;

            if (WindowState == WindowState.Maximized)
                FitMaxSizeToMonitor();

            ApplyCornerPreference();
        }

        /// <summary>
        /// Square corners when maximized, otherwise the cached preference.
        /// </summary>
        private void ApplyCornerPreference()
        {
            WindowEffects.SetCornerPreference(_hwnd,
                WindowState == WindowState.Maximized ? WindowCornerPreference.DoNotRound : _cornerPref);
        }

        /// <summary>
        /// Makes sure MaxWidth/MaxHeight don't clamp the window below the work area of the monitor it's on.
        /// </summary>
        private void FitMaxSizeToMonitor()
        {
            if (!MonitorHelper.TryGetMonitorAreas(_hwnd, out RECT work, out _))
                return;

            // Monitor info is in physical pixels; WPF sizes are in DIPs.
            DpiScale dpi = VisualTreeHelper.GetDpi(this);
            MaxWidth = work.Width / dpi.DpiScaleX;
            MaxHeight = work.Height / dpi.DpiScaleY;
        }

        #region Title bar

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            if (e.ClickCount == 2)
            {
                if (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip)
                    ToggleMaximizedState();
            }
            else
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) => ToggleMaximizedState();

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void ToggleMaximizedState()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion
    }
}
