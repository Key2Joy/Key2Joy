using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Key2Joy.Extensions;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Key2Joy.Util;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Key2Joy.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static Window CurrentWindow { get; private set; }

    private static bool shouldStartMinimized;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        Key2JoyManager.InitSafely(
            OnRunAppCommand,
            () =>
                {
                    var args = Environment.GetCommandLineArgs();

                    ApplicationConfiguration.Initialize();

                    foreach (var arg in args)
                    {
                        if (arg == "--minimized")
                        {
                            shouldStartMinimized = true;
                        }
                    }

                    ShowForm(GetStartupWindow());
                }
            );
    }

    public static void ShowForm(Window window)
    {
        if (window is MainWindow)
        {
            MappingProfile.ExtractDefaultIfNotExists();
            var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();

            try
            {
                gamePadService.Initialize();
            }
            catch
            {
                var dialog = new ContentDialog
                {
                    Title = "Key2Joy",
                    Content = "Failed to initialize the virtual gamepad driver. Please ensure that the SCP Virtual Bus Driver is correctly installed.",
                    CloseButtonText = "OK",
                    XamlRoot = window.Content.XamlRoot
                };

                _ = dialog.ShowAsync();

                return;
            }
        }

        window.Activate();

        var oldWindow = CurrentWindow;
        CurrentWindow = window;
        oldWindow?.Close();
    }

    private static Window GetStartupWindow()
    {
        if (ScpBusExtensions.IsDriverInstalled())
        {
            return new MainWindow(shouldStartMinimized);
        }

        // TODO: return new SetupForm();
        return new MainWindow(false);
    }

    private static bool OnRunAppCommand(AppCommand command)
    {
        if (CurrentWindow is IAcceptAppCommands form)
        {
            return form.RunAppCommand(command);
        }

        return false;
    }
}
