using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Key2Joy.Mapping.Actions.Windows;

public abstract class WindowAction : CoreAction
{
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

    public string WindowTitle { get; set; }
    public string ClassName { get; set; }
    public string Executable { get; set; }

    protected WindowAction(string name)
        : base(name)
    { }

    protected IntPtr FindMatchingWindow()
    {
        var result = IntPtr.Zero;

        EnumWindows((hWnd, lParam) =>
        {
            const int nChars = 256;
            var titleBuilder = new StringBuilder(nChars);
            if (GetWindowText(hWnd, titleBuilder, nChars) <= 0)
            {
                return true;
            }

            if (titleBuilder.ToString() != this.WindowTitle)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(this.Executable))
            {
                var exeFileName = GetExecutableFileName(hWnd);
                if (!string.Equals(exeFileName, this.Executable, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            result = hWnd;
            return false;
        }, IntPtr.Zero);

        return result;
    }

    public static string GetExecutableFileName(IntPtr hWnd)
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

    public override string GetNameDisplay() => this.Name.Replace("{0}", this.WindowTitle);

    public override bool Equals(object obj)
    {
        if (obj is not WindowAction action)
        {
            return false;
        }

        return action.WindowTitle == this.WindowTitle
            && action.ClassName == this.ClassName
            && action.Executable == this.Executable;
    }
}
