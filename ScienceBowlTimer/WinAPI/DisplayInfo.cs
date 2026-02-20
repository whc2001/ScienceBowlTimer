using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Screen = System.Windows.Forms.Screen;

namespace ScienceBowlTimer.WinAPI
{
    public static class DisplayInfo
    {
        private enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2
        }

        private const int MONITOR_DEFAULTTONEAREST = 2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(System.Drawing.Point pt, int flags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int flags);

        [System.Runtime.InteropServices.DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hMonitor, DpiType dpiType, out uint dpiX, out uint dpiY);

        public static double GetDPIScaleForScreen(Screen screen)
        {
            try
            {
                // Try to get the DPI for this specific screen
                var hMonitor = MonitorFromPoint(
                    new Point(screen.Bounds.Left + screen.Bounds.Width / 2, screen.Bounds.Top + screen.Bounds.Height / 2),
                    MONITOR_DEFAULTTONEAREST);

                if (GetDpiForMonitor(hMonitor, DpiType.Effective, out uint dpiX, out uint _) == 0)
                {
                    return dpiX / 96.0;
                }
            }
            catch
            {
                // Fallback if the API is not available
            }

            // Fallback to system DPI
            using var graphics = Graphics.FromHwnd(IntPtr.Zero);
            return graphics.DpiX / 96.0;
        }

        public static bool IsDualDisplay()
        {
            return Screen.AllScreens.Length > 1;
        }

        public static Screen GetPrimaryScreen()
        {
            return Screen.AllScreens.First(s => s.Primary);
        }

        public static Screen? GetSecondaryScreen()
        {
            if (!IsDualDisplay())
                return null;
            return Screen.AllScreens.First(s => !s.Primary);
        }

        public static double GetPrimaryScreenDPIScale()
        {
            return GetDPIScaleForScreen(GetPrimaryScreen());
        }

        public static Screen? GetScreenFromPoint(Point point)
        {
            return Screen.FromPoint(point);
        }

        // Get the screen a WPF window is currently on by converting WPF coordinates to physical screen coordinates
        public static Screen? GetScreenFromWindow(System.Windows.Window window)
        {
            // Get the window's handle
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;

            // Get the monitor handle from the window handle
            var hMonitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            // Find which screen corresponds to this monitor
            foreach (var screen in Screen.AllScreens)
            {
                var screenMonitor = MonitorFromPoint(
                    new Point(screen.Bounds.Left + screen.Bounds.Width / 2, 
                             screen.Bounds.Top + screen.Bounds.Height / 2),
                    MONITOR_DEFAULTTONEAREST);

                if (screenMonitor == hMonitor)
                {
                    return screen;
                }
            }

            return null;
        }
    }
}
