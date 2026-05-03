using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.UI.Xaml;
using SimWinInput;

namespace Key2Joy.App;

public sealed partial class SetupWindow : Window
{
    public SetupWindow()
    {
        this.InitializeComponent();

        var appWindow = this.AppWindow;
        appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Overlapped);

        var overlapped = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
        if (overlapped != null)
        {
            overlapped.IsMaximizable = false;
            overlapped.IsResizable = false;
        }
    }

    private void OnRootSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var elm = (FrameworkElement)sender;
        elm.SizeChanged -= this.OnRootSizeChanged;

        // Edit from original: account for scaling
        var scale = elm.XamlRoot.RasterizationScale;
        var height = (int)Math.Ceiling(elm.DesiredSize.Height * scale);
        var width = (int)Math.Ceiling(elm.DesiredSize.Width * scale);

        this.AppWindow.ResizeClient(new(width, height));
        this.Activate();
    }

    private void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ScpDriverInstaller.Install();
            App.ShowMainWindow();
        }
        catch (Win32Exception)
        {
            return;
        }
    }

    private void CreateSystemRestorePointButton_Click(object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo
    {
        FileName = "SystemPropertiesProtection.exe",
        UseShellExecute = true
    });

    private static bool IsRunningAsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static void RestartAsAdministrator()
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = Environment.ProcessPath,
            UseShellExecute = true,
            Verb = "runas"   // triggers UAC prompt
        };

        try
        {
            Process.Start(processInfo);
            Application.Current.Exit();  // close the non-elevated instance
        }
        catch (Win32Exception)
        {
            // user declined the UAC prompt — stay open
        }
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        // Ensure we are running as an administrator, which is required for installing the driver. If not, restart the app with elevated privileges.
        if (!IsRunningAsAdministrator())
        {
            RestartAsAdministrator();
            return;
        }
    }
}
