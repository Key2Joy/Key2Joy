using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Config;
using Key2Joy.Mapping;
using Key2Joy.Util;

namespace Key2Joy.App.Pages;

public class MainMappingPageViewModel
{
    public ObservableCollection<MappedOption> MappedOptions = new();

    public MappingProfile SelectedProfile;

    private readonly ConfigState configState;

    public MainMappingPageViewModel()
    {
        this.configState = ServiceContainer.Get<IConfigManager>()
            .GetConfigState();
    }

    public void Initialize()
    {
        var lastLoadedProfile = MappingProfile.RestoreLastLoaded();

        if (lastLoadedProfile != null)
        {
            this.SetSelectedProfile(lastLoadedProfile);
        }

        // Ensure the manager knows which window handle catches all inputs
        // TODO: Key2JoyManager.Instance.SetHandlerWithInvoke(this);
        Key2JoyManager.Instance.StatusChanged += (s, ev) =>
        {
            //this.SetStatusView(ev.IsEnabled);

            if (ev.Profile != null)
            {
                this.SetSelectedProfile(ev.Profile);
            }
        };
    }

    private void SetSelectedProfile(MappingProfile profile)
    {
        this.SelectedProfile = profile;
        this.configState.LastLoadedProfile = profile.FilePath;

        MappedOptions.Clear();

        foreach (var mappedOption in profile.MappedOptions)
        {
            MappedOptions.Add(mappedOption);
        }
    }
}
