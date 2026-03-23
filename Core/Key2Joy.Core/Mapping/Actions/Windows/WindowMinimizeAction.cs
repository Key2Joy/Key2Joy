using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Contracts.Mapping.Triggers;

namespace Key2Joy.Mapping.Actions.Windows;

[Action(
    Description = "Minimize a Window",
    NameFormat = "Minimize Window '{0}'",
    GroupName = "Windows",
    GroupImage = "application_xp_terminal"
)]
public class WindowMinimizeAction : WindowAction
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_MINIMIZE = 6;

    public WindowMinimizeAction(string name)
        : base(name)
    { }

    /// <markdown-doc>
    /// <parent-name>Windows</parent-name>
    /// <path>Api/Windows</path>
    /// </markdown-doc>
    /// <summary>
    /// Minimize a window by a class name / title.
    /// </summary>
    /// <param name="windowTitle">Window title to find and minimize</param>
    /// <param name="className">Optional window class name</param>
    /// <name>Window.Minimize</name>
    [ExposesScriptingMethod("Window.Minimize")]
    public async void ExecuteForScript(string windowTitle, string className = null)
    {
        this.WindowTitle = windowTitle;
        this.ClassName = className;

        await this.Execute();
    }

    public override Task Execute(AbstractInputBag inputBag = null)
    {
        var hWnd = this.FindMatchingWindow();

        if (hWnd == IntPtr.Zero)
        {
            // Fail silently
            return Task.CompletedTask;
        }

        ShowWindow(hWnd, SW_MINIMIZE);

        return Task.CompletedTask;
    }
}
