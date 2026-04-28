using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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
    TextGlyph = "\uE923"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class WindowActionControl : UserControl, IActionOptionsControl
{
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsWindowVisible(IntPtr hWnd);
    [LibraryImport("user32.dll")]
    private static partial long GetWindowLong(IntPtr hWnd, int nIndex);
    [LibraryImport("kernel32.dll")]
    private static partial IntPtr OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(IntPtr hObject);
    [LibraryImport("user32.dll")]
    private static partial uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
    private static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, ref int lpdwSize);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

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
        var _ = GetWindowThreadProcessId(hWnd, out var pid);

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

    public sealed class WindowInfo(IntPtr handle, string identifier)
    {
        public IntPtr Handle { get; } = handle;
        public string Identifier { get; } = identifier;

        public override string ToString() => this.Identifier;
    }
}
