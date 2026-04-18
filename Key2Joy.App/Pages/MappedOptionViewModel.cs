using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping;

namespace Key2Joy.App.Pages;

public partial class MappedOptionViewModel : ObservableObject
{
    public MappedOption Option { get; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public AbstractTrigger Trigger => this.Option.Trigger;

    public AbstractAction Action => this.Option.Action;

    public bool HasChildren => this.Children.Count > 0;

    public IReadOnlyList<MappedOptionViewModel> Children { get; }

    public MappedOptionViewModel(MappedOption option)
    {
        this.Option = option;
        this.Children = option.Children
            .Select(static child => new MappedOptionViewModel(child))
            .ToList();
    }
}
