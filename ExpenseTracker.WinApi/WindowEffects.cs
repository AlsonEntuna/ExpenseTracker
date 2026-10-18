using System;
using System.Runtime.InteropServices;

namespace ExpenseTracker.WinApi
{
    /// <summary>
    /// DWM window corner preference (DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE).
    /// </summary>
    public enum WindowCornerPreference
    {
        Default = 0,
        DoNotRound = 1,
        Round = 2,
        RoundSmall = 3
    }

    /// <summary>
    /// Window visual effects: Mica, acrylic blur and Win11 rounded corners.
    /// </summary>
    public static class WindowEffects
    {
        public static void SetCornerPreference(IntPtr hwnd, WindowCornerPreference preference)
        {
            int value = (int)preference;
            NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_WINDOW_CORNER_PREFERENCE, ref value, sizeof(int));
        }

        public static void EnableMica(IntPtr hwnd)
        {
            int trueValue = 1;
            NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_MICA_EFFECT, ref trueValue, sizeof(int));
        }

        public static void EnableAcrylicBlur(IntPtr hwnd)
        {
            var accent = new NativeMethods.AccentPolicy
            {
                AccentState = NativeMethods.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                //GradientColor = 0x99FFFFFF // ARGB (adjust opacity here)
            };

            int accentStructSize = Marshal.SizeOf(accent);
            IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
            try
            {
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new NativeMethods.WindowCompositionAttributeData
                {
                    Attribute = NativeMethods.WCA_ACCENT_POLICY,
                    SizeOfData = accentStructSize,
                    Data = accentPtr
                };

                NativeMethods.SetWindowCompositionAttribute(hwnd, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(accentPtr);
            }
        }
    }
}
