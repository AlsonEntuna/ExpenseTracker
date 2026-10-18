using System;
using System.Runtime.InteropServices;

namespace ExpenseTracker.WinApi
{
    public static class MonitorHelper
    {
        /// <summary>
        /// Gets the work area (screen minus taskbar) and full bounds, in physical pixels,
        /// of the monitor the window is on.
        /// </summary>
        public static bool TryGetMonitorAreas(IntPtr hwnd, out RECT workArea, out RECT monitorArea)
        {
            workArea = default;
            monitorArea = default;

            IntPtr monitor = NativeMethods.MonitorFromWindow(hwnd, NativeMethods.MONITOR_DEFAULTTONEAREST);
            if (monitor == IntPtr.Zero)
                return false;

            var info = new NativeMethods.MONITORINFO { cbSize = Marshal.SizeOf<NativeMethods.MONITORINFO>() };
            if (!NativeMethods.GetMonitorInfo(monitor, ref info))
                return false;

            workArea = info.rcWork;
            monitorArea = info.rcMonitor;
            return true;
        }

        /// <summary>
        /// Window message hook (HwndSourceHook signature) that keeps a borderless window
        /// from covering the taskbar when maximized.
        /// </summary>
        public static IntPtr MaximizeToWorkAreaHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_GETMINMAXINFO
                && TryGetMonitorAreas(hwnd, out RECT work, out RECT monitor))
            {
                var mmi = Marshal.PtrToStructure<NativeMethods.MINMAXINFO>(lParam);

                mmi.ptMaxPosition.X = Math.Abs(work.Left - monitor.Left);
                mmi.ptMaxPosition.Y = Math.Abs(work.Top - monitor.Top);
                mmi.ptMaxSize.X = Math.Abs(work.Width);
                mmi.ptMaxSize.Y = Math.Abs(work.Height);

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            return IntPtr.Zero;
        }
    }
}
