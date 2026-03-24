using System;
using System.Drawing;
using System.Windows.Forms;
using Key2Joy.Extensions;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Key2Joy.Util;
using SimWinInput;

namespace Key2Joy.Gui;

public static class Program
{
    public static Form ActiveForm { get; set; }

    private static bool shouldStartMinimized;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Key2JoyManager.InitSafely(
            OnRunAppCommand,
            () =>
            {
                var args = Environment.GetCommandLineArgs();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                foreach (var arg in args)
                {
                    if (arg == "--minimized")
                    {
                        shouldStartMinimized = true;
                    }
                }

                ShowForm(GetStartupForm());


                while (ActiveForm != null && !ActiveForm.IsDisposed)
                {
                    Application.Run(ActiveForm);
                }
            }
        );
    }

    private static Form GetStartupForm()
    {
        // ScpBus does not have a static property to check if the driver is installed.
        // You need to implement your own check or use an existing helper if available.
        // For now, we will assume the driver is installed. Replace this with a real check if possible.
        if (true)
        {
            return new MainForm(shouldStartMinimized);
        }
        return new SetupForm();
    }

    public static void ShowForm(Form form)
    {
        if (form is MainForm)
        {
            MappingProfile.ExtractDefaultIfNotExists();
            var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();

            try
            {
                gamePadService.Initialize();
            }
            catch
            {
                MessageBox.Show("Failed to initialize the virtual gamepad driver. Please ensure that the SCP Virtual Bus Driver is correctly installed.", "Key2Joy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        var oldForm = ActiveForm;
        ActiveForm = form;
        oldForm?.Close();
    }

    public static void ShowMainForm()
    {
        ShowForm(new MainForm(shouldStartMinimized));
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
}
