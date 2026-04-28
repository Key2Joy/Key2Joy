using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Windows;
using Microsoft.UI.Xaml.Controls;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;
using Windows.Win32.UI.WindowsAndMessaging;

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

        PInvoke.EnumWindows((hWnd, _) =>
        {
            if (!PInvoke.IsWindowVisible(hWnd))
            {
                return true;
            }

            // Stack-allocate a buffer for the window title
            Span<char> titleBuffer = stackalloc char[256];
            int length;

            unsafe
            {
                fixed (char* pTitle = titleBuffer)
                {
                    length = PInvoke.GetWindowText(hWnd, pTitle, titleBuffer.Length);
                }
            }

            if (length == 0)
            {
                return true;
            }

            var title = new string(titleBuffer[..length]);
            var exStyle = (WINDOW_EX_STYLE)PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

            if (
                exStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_TOOLWINDOW)
                || exStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_NOACTIVATE)
            )
            {
                return true;
            }

            var exe = GetExecutableFileName(hWnd);
            var identifier = string.IsNullOrEmpty(exe)
                ? title
                : $"{exe}{WindowAction.IdentifierSeparator}{title}";

            this.AvailableWindows.Add(new WindowInfo(hWnd, identifier));

            return true;
        }, IntPtr.Zero);
    }

    private static string GetExecutableFileName(IntPtr hWnd)
    {
        uint pid;

        unsafe
        {
            _ = PInvoke.GetWindowThreadProcessId((HWND)hWnd, &pid);
        }

        using var hProcess = PInvoke.OpenProcess_SafeHandle(
            PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION,
            false,
            pid);

        if (hProcess.IsInvalid)
        {
            return string.Empty;
        }

        Span<char> buffer = stackalloc char[1024];
        var size = (uint)buffer.Length;

        if (
            PInvoke.QueryFullProcessImageName(
                hProcess,
                PROCESS_NAME_FORMAT.PROCESS_NAME_WIN32,
                buffer,
                ref size
            )
        )
        {
            return System.IO.Path.GetFileName(new string(buffer[..(int)size]));
        }

        return string.Empty;
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
