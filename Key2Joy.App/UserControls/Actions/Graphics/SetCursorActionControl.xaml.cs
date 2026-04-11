using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Graphics;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Key2Joy.App.UserControls.Actions.Graphics;

[MappingControl(
    ForType = typeof(SetCursorAction),
    ImageResourceName = "ms-appx:///Assets/Icons/cursor.png"
)]
[ObservableObject]
public sealed partial class SetCursorActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler? OptionsChanged;
    public ObservableCollection<string> AvailableCursors { get; } = [];

    [ObservableProperty]
    public partial string SelectedCursor { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string CursorFilePath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsFileMode { get; set; }

    public SetCursorActionControl()
    {
        this.InitializeComponent();
        this.LoadCursors();
    }

    private void LoadCursors()
    {
        this.AvailableCursors.Clear();
        foreach (var key in SetCursorAction.SystemCursorIds.Keys)
        {
            this.AvailableCursors.Add(key);
        }
        if (this.AvailableCursors.Count > 0)
        {
            this.SelectedCursor = this.AvailableCursors[0];
        }
    }

    partial void OnSelectedCursorChanged(string value)
    {
        this.IsFileMode = value == "FILE";
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnCursorFilePathChanged(string value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    private async Task BrowseForCursorFileAsync()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".cur");
        picker.FileTypeFilter.Add(".ani");
        picker.FileTypeFilter.Add(".dll");
        picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.CurrentWindow));

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            this.CursorFilePath = file.Path;
        }
    }

    private void BrowseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        => _ = this.BrowseForCursorFileAsync();

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (SetCursorAction)action;
        this.CursorFilePath = thisAction.CursorFilePath;
        if (!string.IsNullOrEmpty(thisAction.CursorName)
            && this.AvailableCursors.Contains(thisAction.CursorName))
        {
            this.SelectedCursor = thisAction.CursorName;
        }
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (SetCursorAction)action;
        thisAction.CursorName = this.SelectedCursor;
        thisAction.CursorFilePath = this.CursorFilePath;
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action) => true;
}
