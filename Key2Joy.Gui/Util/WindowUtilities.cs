using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Key2Joy.Mapping.Actions.Windows;

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

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_TOOLWINDOW = 0x00000080L;
    private const long WS_EX_NOACTIVATE = 0x08000000L;
    private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

    public static List<WindowInfo> GetTopLevelWindows()
    {
        var windows = new List<WindowInfo>();

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

            windows.Add(new WindowInfo(hWnd, WindowAction.BuildIdentifier(GetExecutableFileName(hWnd), titleBuilder.ToString())));
            return true;
        }, IntPtr.Zero);

        return windows;
    }

    private static string GetExecutableFileName(IntPtr hWnd)
    {
        GetWindowThreadProcessId(hWnd, out var processId);
        if (processId == 0)
        {
            return null;
        }

        var hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, processId);
        if (hProcess == IntPtr.Zero)
        {
            return null;
        }

        try
        {
            uint size = 1024;
            var exeBuilder = new StringBuilder((int)size);
            if (QueryFullProcessImageName(hProcess, 0, exeBuilder, ref size))
            {
                return Path.GetFileName(exeBuilder.ToString());
            }

            return null;
        }
        finally
        {
            CloseHandle(hProcess);
        }
    }

    public class WindowInfo
    {
        public IntPtr Handle { get; }
        public string Identifier { get; }

        public WindowInfo(IntPtr handle, string identifier)
        {
            this.Handle = handle;
            this.Identifier = identifier;
        }

        public override string ToString() => this.Identifier;
    }
}
