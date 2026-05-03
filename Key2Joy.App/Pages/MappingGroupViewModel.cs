using System.Collections.ObjectModel;

namespace Key2Joy.App.Pages;

public class MappingGroupViewModel
{
    public string GroupName { get; set; } = string.Empty;

    public bool IsHeaderVisible { get; set; }

    public ObservableCollection<MappedOptionViewModel> Items { get; } = [];
}
