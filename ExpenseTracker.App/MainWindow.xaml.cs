using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ExpenseTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
           
            AllowsTransparency = true;
            // TODO: move this to the themes
            SourceInitialized += (_, _) => EnableAcrylic();

            _vm = DataContext as MainWindowViewModel;
        }
        #region Blur
        private void EnableAcrylic()
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            // Mica
            int trueValue = 1;

            // Enable Mica
            DwmSetWindowAttribute(hwnd, 1029, ref trueValue, sizeof(int));

            var accent = new AccentPolicy
            {
                AccentState = 3, // ACCENT_ENABLE_ACRYLICBLURBEHIND
                //GradientColor = 0x99FFFFFF // ARGB (adjust opacity here)
            };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = 19, // WCA_ACCENT_POLICY
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(hwnd, ref data);

            // Win11 native round corner attribute
            int windowRoundCornerAttribute = 2; // DWMWCP_ROUND
            DwmSetWindowAttribute(hwnd, (int)DwmWindowAttribute.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowRoundCornerAttribute, sizeof(int));

            Marshal.FreeHGlobal(accentPtr);
        }

        internal enum DwmWindowAttribute
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public int AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public int Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(
            IntPtr hwnd,
            ref WindowCompositionAttributeData data);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int dwAttribute,
        ref int pvAttribute,
        int cbAttribute);

        #endregion
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _vm?.SaveData();
        }

        private void TitleBarGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Open_Click(object sender, RoutedEventArgs e)
        {
            _vm?.OpenVariableExpense();
        }

        private void Btn_New_Click(object sender, RoutedEventArgs e)
        {
            _vm?.CreateVariableExpense();
        }

        private void Btn_Tools_Click(object sender, RoutedEventArgs e)
        {
            _vm?.OpenToolsPanel();
        }

        private void Button_Maximize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                     ? WindowState.Normal
                     : WindowState.Maximized;

            Btn_Maximize.Content = WindowState == WindowState.Maximized ? "❐" : "☐";
        }
        private void Button_Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Maximized)
            {
                var screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);

                MaxHeight = screen.WorkingArea.Height;
                MaxWidth = screen.WorkingArea.Width;
            }
        }
    }
}
