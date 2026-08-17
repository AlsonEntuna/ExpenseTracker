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

        private nint _hwndHandle;

        // DWM Window Corner Preference
        // 0 - Default
        // 1 - DoNotRound
        // 2 - Round
        // 3 - RoundSmall
        private int _cornerPref = 0;
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
            _hwndHandle = new WindowInteropHelper(this).Handle;

            // Mica
            int trueValue = 1;

            // Enable Mica
            DwmSetWindowAttribute(_hwndHandle, 1029, ref trueValue, sizeof(int));

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

            SetWindowCompositionAttribute(_hwndHandle, ref data);

            // Win11 native round corner attribute
            int windowRoundCornerAttribute = 2; // DWMWCP_ROUND
            DwmSetWindowAttribute(_hwndHandle, (int)DwmWindowAttribute.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowRoundCornerAttribute, sizeof(int));
            _cornerPref = windowRoundCornerAttribute;

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
                ToggleMaximizedState();
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
            ToggleMaximizedState();
        }

        private void ToggleMaximizedState()
        {
            int pref = 1; // DoNotRound
            if (WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
                pref = _cornerPref; // cached corner pref
            }

            // Apply the corner pref
            DwmSetWindowAttribute(_hwndHandle
                , (int)DwmWindowAttribute.DWMWA_WINDOW_CORNER_PREFERENCE
                , ref pref
                , Marshal.SizeOf(pref));

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

        // TODO: put or separate this into a separate WinApi Lib
        #region Maximize Fix

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
        }
        private const int WM_GETMINMAXINFO = 0x0024;

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                // We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
                // including the task bar.
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                // Adjust the maximized size and position to fit the work area of the correct monitor
                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new MONITORINFO();
                    monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                    GetMonitorInfo(monitor, ref monitorInfo);
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                }

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            return IntPtr.Zero;
        }
        #endregion
    }

}
