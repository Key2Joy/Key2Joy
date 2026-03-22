using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Key2Joy.Gui.Util;

internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

internal class WindowUtilities
{
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern long GetWindowLong(IntPtr hWnd, int nIndex);

    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_TOOLWINDOW = 0x00000080L;
    private const long WS_EX_NOACTIVATE = 0x08000000L;

    public static List<(IntPtr Handle, string Title)> GetTopLevelWindows()
    {
        var windows = new List<(IntPtr Handle, string Title)>();

        EnumWindows((hWnd, lParam) =>
        {
            // Must be visible
            if (!IsWindowVisible(hWnd))
            {
                return true;
            }

            // Must have a title
            const int nChars = 256;
            var titleBuilder = new StringBuilder(nChars);
            if (GetWindowText(hWnd, titleBuilder, nChars) <= 0)
            {
                return true;
            }

            // Skip tool windows (system tray popups, floating toolbars, etc.)
            // Skip non-activatable windows (overlays, always-on-top HUDs, etc.)
            var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            if ((exStyle & WS_EX_TOOLWINDOW) != 0 || (exStyle & WS_EX_NOACTIVATE) != 0)
            {
                return true;
            }

            windows.Add((hWnd, titleBuilder.ToString()));
            return true;
        }, IntPtr.Zero);

        return windows;
    }
}
