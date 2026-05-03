using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Contracts.Mapping.Triggers;

namespace Key2Joy.Mapping.Actions.Windows;

[Action(
    Description = "Focus a Window",
    NameFormat = "Focus Window '{0}'",
    GroupName = "Windows"
)]
public class WindowFocusAction : WindowAction
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private const int SW_RESTORE = 9;

    public WindowFocusAction(string name)
        : base(name)
    { }

    /// <markdown-doc>
    /// <parent-name>Windows</parent-name>
    /// <path>Api/Windows</path>
    /// </markdown-doc>
    /// <summary>
    /// Focus (bring to front) a window by a class name / title.
    /// </summary>
    /// <param name="windowTitle">Window title to find and focus</param>
    /// <param name="className">Optional window class name</param>
    /// <name>Window.Focus</name>
    [ExposesScriptingMethod("Window.Focus")]
    public async void ExecuteForScript(string windowTitle, string className = null)
    {
        this.WindowIdentifier = windowTitle;
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

        ShowWindow(hWnd, SW_RESTORE);
        SetForegroundWindow(hWnd);

        return Task.CompletedTask;
    }
}
