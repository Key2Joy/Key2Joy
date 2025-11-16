using System;
using System.Drawing;
using System.Windows.Forms;
using CommonServiceLocator;
using Key2Joy.Extensions;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Key2Joy.Plugins;

namespace Key2Joy.Gui;

public static class Program
{
    public static Form ActiveForm { get; set; }
    public static PluginSet Plugins { get; private set; }

    private static bool shouldStartMaximized;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Key2JoyManager.InitSafely(
            OnRunAppCommand,
            (plugins) =>
            {
                var args = Environment.GetCommandLineArgs();

                Plugins = plugins;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                shouldStartMaximized = false;

                foreach (var arg in args)
                {
                    if (arg == "--minimized")
                    {
                        shouldStartMaximized = true;
                    }
                }

                ActiveForm = GetStartupForm();

                while (ActiveForm != null && !ActiveForm.IsDisposed)
                {
                    Application.Run(ActiveForm);
                }
            }
        );

        Plugins.Dispose();
    }

    private static Form GetStartupForm()
    {
        if (ScpBusExtensions.IsDriverInstalled())
        {
            return new MainForm(shouldStartMaximized);
        }
        return new SetupForm();
    }

    internal static void GoToNextForm(Form form)
    {
        var oldForm = ActiveForm;
        ActiveForm = form;

        oldForm.Close();
    }

    internal static Bitmap ResourceBitmapFromName(string name)
    {
        var rm = Properties.Resources.ResourceManager;
        return (Bitmap)rm.GetObject(name);
    }

    private static bool OnRunAppCommand(AppCommand command)
    {
        if (ActiveForm is IAcceptAppCommands form)
        {
            return form.RunAppCommand(command);
        }

        return false;
    }

    public static void GoToMainForm()
    {
        MappingProfile.ExtractDefaultIfNotExists();
        var gamePadService = ServiceLocator.Current.GetInstance<ISimulatedGamePadService>();

        try
        {
            gamePadService.Initialize();
        }
        catch
        {
            Application.Exit();
            return;
        }

        GoToNextForm(new MainForm(shouldStartMaximized));
    }
}
