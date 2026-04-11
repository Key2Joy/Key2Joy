using System;
using System.Diagnostics;
using System.IO;
using Key2Joy.App.Pages;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using Windows.Foundation;
using WinRT.Interop;

namespace Key2Joy.App;

public sealed partial class MainWindow : Window, IAcceptAppCommands
{
    private MainMappingPage? _mappingPage;

    public MainWindow(bool shouldStartMinimized)
    {
        this.InitializeComponent();

        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(this.TitleBar);

        this.MainFrame.Navigate(typeof(Pages.MainMappingPage));
    }

    public bool RunAppCommand(AppCommand command)
    {
        if (this.MainFrame.Content is IAcceptAppCommands page)
        {
            return page.RunAppCommand(command);
        }

        return false;
    }

    private void MainFrame_Navigated(object sender, NavigationEventArgs e)
    {
        if (this.MainFrame.Content is MainMappingPage page)
        {
            this._mappingPage = page;
        }
    }

    private void MenuNewProfile_Click(object sender, RoutedEventArgs e)
        => this._mappingPage?.CreateNewProfile(" - Copy");

    private async void MenuLoadProfile_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker(this.AppWindow.Id)
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        picker.FileTypeFilter.Add(MappingProfile.EXTENSION);
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));

        var file = await picker.PickSingleFileAsync();
        if (file == null)
        {
            return;
        }

        var profile = MappingProfile.Load(file.Path);
        if (profile == null)
        {
            await ShowError(
                this.Content.XamlRoot,
                "Failed to load profile!",
                "The selected profile was corrupt!\n\nPlease help us by reporting this bug on GitHub."
            );
            return;
        }

        this._mappingPage?.SetSelectedProfile(profile);
    }

    private void MenuOpenProfileFolder_Click(object sender, RoutedEventArgs e)
    {
        var profile = this._mappingPage?.ViewModel.SelectedProfile;
        if (profile == null)
        {
            Process.Start(new ProcessStartInfo { FileName = MappingProfile.GetSaveDirectory(), UseShellExecute = true });
            return;
        }

        Process.Start("explorer.exe", $"/select, \"{profile.FilePath}\"");
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e)
        => Application.Current.Exit();

    //private void MenuGamePadPressRelease_Click(object sender, RoutedEventArgs e)
    //    => _mappingPage?.AddAllGamePadMappings(includePressRelease: true);

    //private void MenuGamePadPress_Click(object sender, RoutedEventArgs e)
    //    => _mappingPage?.AddAllGamePadMappings(pressOnly: true);

    //private void MenuGamePadRelease_Click(object sender, RoutedEventArgs e)
    //    => _mappingPage?.AddAllGamePadMappings(releaseOnly: true);

    //private void MenuKeyboardPressRelease_Click(object sender, RoutedEventArgs e)
    //    => _mappingPage?.AddAllKeyboardMappings(includePressRelease: true);

    //private void MenuKeyboardPress_Click(object sender, RoutedEventArgs e)
    //    => _mappingPage?.AddAllKeyboardMappings(pressOnly: true);

    //private void MenuKeyboardRelease_Click(object sender, RoutedEventArgs e)
    //    => _mappingPage?.AddAllKeyboardMappings(releaseOnly: true);

    private void MenuTestKeyboard_Click(object sender, RoutedEventArgs e)
        => OpenUrl("https://devicetests.com/keyboard-tester");

    private void MenuTestMouse_Click(object sender, RoutedEventArgs e)
        => OpenUrl("https://devicetests.com/mouse-test");

    private void MenuTestController_Click(object sender, RoutedEventArgs e)
        => OpenUrl("https://devicetests.com/controller-tester");

    private async void MenuConfig_Click(object sender, RoutedEventArgs e)
        => this.NavigateWithBackButton(typeof(ConfigPage));

    private async void MenuViewLog_Click(object sender, RoutedEventArgs e)
    {
        var logFile = Key2Joy.Contracts.Output.GetLogPath();
        if (!File.Exists(logFile))
        {
            await ShowError(
                this.Content.XamlRoot,
                "Log file not found",
                "The log file does not exist yet. Please wait for the app to write to it."
            );
            return;
        }

        Process.Start(new ProcessStartInfo { FileName = logFile, UseShellExecute = true });
    }

    private void MenuReportProblem_Click(object sender, RoutedEventArgs e)
        => OpenUrl("https://github.com/Key2Joy/Key2Joy/issues");

    private void MenuViewSource_Click(object sender, RoutedEventArgs e)
        => OpenUrl("https://github.com/Key2Joy/Key2Joy");

    private async void MenuAbout_Click(object sender, RoutedEventArgs e)
        => this.NavigateWithBackButton(typeof(AboutPage));

    private void NavigateWithBackButton(Type type)
    {
        this.MainFrame.Navigate(type);
        this.TitleBar.IsBackButtonVisible = true;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        this.MainFrame.GoBack();

        if (this.MainFrame.CanGoBack)
        {
            return;
        }

        this.TitleBar.IsBackButtonVisible = false;
    }

    private static IAsyncOperation<ContentDialogResult> ShowError(XamlRoot root, string title, string message)
    {
        var contentDialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = root
        };

        return contentDialog.ShowAsync();
    }

    private static void OpenUrl(string url)
        => Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
}
