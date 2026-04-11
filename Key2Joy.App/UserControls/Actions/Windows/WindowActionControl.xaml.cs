using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Windows;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Windows;

[MappingControl(
    ForTypes = new[] { typeof(WindowMinimizeAction), typeof(WindowFocusAction) },
    ImageResourceName = "ms-appx:///Assets/Icons/application_xp_terminal.png"
)]
[ObservableObject]
public sealed partial class WindowActionControl : UserControl, IActionOptionsControl
{
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern long GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);
    [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
    private static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, ref int lpdwSize);
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_TOOLWINDOW = 0x80L;
    private const long WS_EX_NOACTIVATE = 0x8000000L;
    private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

    public event EventHandler? OptionsChanged;
    public ObservableCollection<WindowInfo> AvailableWindows { get; } = [];

    [ObservableProperty]
    public partial string WindowIdentifier { get; set; } = string.Empty;

    [ObservableProperty]
    public partial WindowInfo? SelectedWindow { get; set; }

    public WindowActionControl()
    {
        this.InitializeComponent();
        this.LoadWindows();
    }

    private void LoadWindows()
    {
        this.AvailableWindows.Clear();
        EnumWindows((hWnd, _) =>
        {
            if (!IsWindowVisible(hWnd))
            {
                return true;
            }

            var title = new StringBuilder(256);
            if (GetWindowText(hWnd, title, title.Capacity) == 0 || title.Length == 0)
            {
                return true;
            }

            var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            if ((exStyle & WS_EX_TOOLWINDOW) != 0 || (exStyle & WS_EX_NOACTIVATE) != 0)
            {
                return true;
            }

            var exe = GetExecutableFileName(hWnd);
            var identifier = string.IsNullOrEmpty(exe)
                ? title.ToString()
                : $"{exe}{WindowAction.IdentifierSeparator}{title}";

            this.AvailableWindows.Add(new WindowInfo(hWnd, identifier));

            return true;
        }, IntPtr.Zero);
    }

    private static string GetExecutableFileName(IntPtr hWnd)
    {
        GetWindowThreadProcessId(hWnd, out var pid);

        var hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);

        if (hProcess == IntPtr.Zero)
        {
            return string.Empty;
        }

        try
        {
            var sb = new StringBuilder(1024);
            var size = sb.Capacity;

            try
            {
                if (QueryFullProcessImageName(hProcess, 0, sb, ref size))
                {
                    return System.IO.Path.GetFileName(sb.ToString());
                }
            }
            catch (EntryPointNotFoundException) { }

            return string.Empty;
        }
        finally
        {
            CloseHandle(hProcess);
        }
    }

    partial void OnWindowIdentifierChanged(string value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSelectedWindowChanged(WindowInfo? value)
    {
        if (value != null)
        {
            this.WindowIdentifier = value.Identifier;
        }
    }

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (WindowAction)action;
        this.WindowIdentifier = thisAction.WindowIdentifier ?? string.Empty;
        foreach (var window in this.AvailableWindows)
        {
            if (window.Identifier == thisAction.WindowIdentifier)
            {
                this.SelectedWindow = window;
                break;
            }
        }
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (WindowAction)action;
        thisAction.WindowIdentifier = this.WindowIdentifier;
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action)
    {
        if (!string.IsNullOrWhiteSpace(this.WindowIdentifier))
        {
            return true;
        }

        _ = this.ShowCannotSaveDialogAsync();
        return false;
    }

    private async Task ShowCannotSaveDialogAsync()
    {
        var dialog = new ContentDialog
        {
            Title = "Cannot save!",
            Content = "No window identifier has been set.",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot,
        };
        await dialog.ShowAsync();
    }

    public sealed class WindowInfo
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
