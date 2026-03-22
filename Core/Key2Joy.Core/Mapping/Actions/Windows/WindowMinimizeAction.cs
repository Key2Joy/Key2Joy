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
public class WindowMinimizeAction : CoreAction
{
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    private static extern IntPtr FindWindow(string className, string windowTitle);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_MINIMIZE = 6;

    public string WindowTitle { get; set; }
    public string ClassName { get; set; }

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
        var hWnd = FindWindow(this.ClassName, this.WindowTitle);

        if (hWnd == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Window '{this.WindowTitle}' not found.");
        }

        ShowWindow(hWnd, SW_MINIMIZE);

        return Task.CompletedTask;
    }

    public override string GetNameDisplay() => this.Name.Replace("{0}", this.WindowTitle);

    public override bool Equals(object obj)
    {
        if (obj is not WindowMinimizeAction action)
        {
            return false;
        }

        return action.WindowTitle == this.WindowTitle
            && action.ClassName == this.ClassName;
    }
}
