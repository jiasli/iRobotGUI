// Reference http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=17266
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace iRobotGUI.Controls
{
    class MouseUtilities
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hwnd, ref Win32Point pt);

        /// Returns the mouse cursor location.
        public static Point GetMousePosition(Visual relativeTo)
        {
            Win32Point mouse = new Win32Point();
            GetCursorPos(ref mouse);
            return relativeTo.PointFromScreen(new Point((double)mouse.X, (double)mouse.Y));
        }
    }
}
