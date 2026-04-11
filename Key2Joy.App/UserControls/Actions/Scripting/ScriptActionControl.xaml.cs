using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Scripting;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Key2Joy.App.UserControls.Actions.Scripting;

[MappingControl(
    ForType = typeof(LuaScriptAction),
    ImageResourceName = "ms-appx:///Assets/Icons/script_code.png"
)]
[ObservableObject]
public sealed partial class ScriptActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler? OptionsChanged;

    [ObservableProperty]
    public partial bool IsScriptPath { get; set; }

    public bool IsInlineScript => !this.IsScriptPath;

    [ObservableProperty]
    public partial string Script { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ScriptFilePath { get; set; } = string.Empty;

    private bool _securityWarningAccepted;

    public ScriptActionControl()
        => this.InitializeComponent();

    partial void OnIsScriptPathChanged(bool value)
    {
        this.OnPropertyChanged(nameof(IsInlineScript));
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnScriptChanged(string value)
    {
        _securityWarningAccepted = false;
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnScriptFilePathChanged(string value)
    {
        _securityWarningAccepted = false;
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task BrowseForScriptFileAsync()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".lua");
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.CurrentWindow));

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            this.ScriptFilePath = file.Path;
        }
    }

    private void BrowseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        => _ = this.BrowseForScriptFileAsync();

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (BaseScriptAction)action;
        this.IsScriptPath = thisAction.IsScriptPath;
        if (thisAction.IsScriptPath)
        {
            this.ScriptFilePath = thisAction.Script ?? string.Empty;
        }
        else
        {
            this.Script = thisAction.Script ?? string.Empty;
        }
        this._securityWarningAccepted = false;
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (BaseScriptAction)action;
        thisAction.IsScriptPath = this.IsScriptPath;
        thisAction.Script = this.IsScriptPath ? this.ScriptFilePath : this.Script;
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action)
    {
        if (this._securityWarningAccepted)
        {
            return true;
        }

        _ = this.ShowSecurityWarningAsync();
        return false;
    }

    private async Task ShowSecurityWarningAsync()
    {
        var dialog = new ContentDialog
        {
            Title = "Security Warning",
            Content = "Scripts can execute arbitrary code on your machine. Only use scripts from sources you trust. Do you want to continue?",
            PrimaryButtonText = "Yes, I trust this script",
            CloseButtonText = "No",
            XamlRoot = this.XamlRoot,
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            this._securityWarningAccepted = true;
        }
    }
}
