using System;
using Key2Joy.Extensions;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Key2Joy.Util;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static Window CurrentWindow { get; private set; }

    private static bool shouldStartMinimized;
    private static Action _cleanupHandle;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="_">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs _)
    {
        _cleanupHandle = Key2JoyManager.InitHandle(OnRunAppCommand);
        var args = Environment.GetCommandLineArgs();

        ApplicationConfiguration.Initialize();

        foreach (var arg in args)
        {
            if (arg == "--minimized")
            {
                shouldStartMinimized = true;
            }
        }

        ShowWindow(GetStartupWindow());
    }

    public static void ShowWindow(Window window)
    {
        if (window is MainWindow)
        {
            MappingProfile.ExtractDefaultIfNotExists();
            var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();

            window.Closed += (_, _) => _cleanupHandle();

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

    public static void ShowMainWindow() => ShowWindow(new MainWindow(shouldStartMinimized));

    private static Window GetStartupWindow()
    {
        if (ScpBusExtensions.IsDriverInstalled())
        {
            return new MainWindow(shouldStartMinimized);
        }

        return new SetupWindow();
    }

    private static bool OnRunAppCommand(AppCommand command)
    {
        if (CurrentWindow is IAcceptAppCommands window)
        {
            return window.RunAppCommand(command);
        }

        return false;
    }
}
