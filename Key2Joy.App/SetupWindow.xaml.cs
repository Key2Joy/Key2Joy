using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SimWinInput;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Key2Joy.App;

public sealed partial class SetupWindow : Window
{
    public SetupWindow()
    {
        InitializeComponent();

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
        elm.SizeChanged -= OnRootSizeChanged;

        // Edit from original: account for scaling
        var scale = elm.XamlRoot.RasterizationScale;
        var height = (int)Math.Ceiling(elm.DesiredSize.Height * scale);
        var width = (int)Math.Ceiling(elm.DesiredSize.Width * scale);

        AppWindow.ResizeClient(new(width, height));
        Activate();
    }

    private void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        if (!IsRunningAsAdministrator())
        {
            RestartAsAdministrator();
            return;
        }

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

    private void CreateSystemRestorePointButton_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "SystemPropertiesProtection.exe",
            UseShellExecute = true
        });
    }

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
}
