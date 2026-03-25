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

    public string WindowIdentifier { get; set; }
    public string ClassName { get; set; }

    protected WindowAction(string name)
        : base(name)
    { }

    public const string IdentifierSeparator = " - ";

    public static string BuildIdentifier(string executable, string title)
    {
        if (string.IsNullOrEmpty(executable))
        {
            return title;
        }

        return $"{executable}{IdentifierSeparator}{title}";
    }

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

            var title = titleBuilder.ToString();
            var exeFileName = GetExecutableFileName(hWnd);
            var identifier = BuildIdentifier(exeFileName, title);

            if (!string.Equals(identifier, this.WindowIdentifier, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(title, this.WindowIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                return true;
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

    public override string GetNameDisplay() => this.Name.Replace("{0}", this.WindowIdentifier);

    public override bool Equals(object obj)
    {
        if (obj is not WindowAction action)
        {
            return false;
        }

        return action.WindowIdentifier == this.WindowIdentifier
            && action.ClassName == this.ClassName;
    }
}
