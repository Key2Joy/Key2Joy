using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Key2Joy.App.UserControls;

public sealed partial class MappingControl : UserControl
{
    public static readonly DependencyProperty TriggersAvailableProperty =
        DependencyProperty.Register(
            nameof(TriggersAvailable),
            typeof(IEnumerable<string>),
            typeof(MappingControl),
            new PropertyMetadata(null));

    public IEnumerable<string> TriggersAvailable
    {
        get => (IEnumerable<string>)GetValue(TriggersAvailableProperty);
        set => SetValue(TriggersAvailableProperty, value);
    }

    public static readonly DependencyProperty TriggerSelectedProperty =
        DependencyProperty.Register(
            nameof(TriggerSelected),
            typeof(string),
            typeof(MappingControl),
            new PropertyMetadata(null));

    public string TriggerSelected
    {
        get => (string)GetValue(TriggerSelectedProperty);
        set => SetValue(TriggerSelectedProperty, value);
    }

    public static readonly DependencyProperty TriggerContentProperty =
        DependencyProperty.Register(
            nameof(TriggerContent),
            typeof(object),
            typeof(MappingControl),
            new PropertyMetadata(null));

    public object TriggerContent
    {
        get => GetValue(TriggerContentProperty);
        set => SetValue(TriggerContentProperty, value);
    }

    public static readonly DependencyProperty ActionsAvailableProperty =
        DependencyProperty.Register(
            nameof(ActionsAvailable),
            typeof(IEnumerable<string>),
            typeof(MappingControl),
            new PropertyMetadata(null));

    public IEnumerable<string> ActionsAvailable
    {
        get => (IEnumerable<string>)GetValue(ActionsAvailableProperty);
        set => SetValue(ActionsAvailableProperty, value);
    }

    public static readonly DependencyProperty ActionSelectedProperty =
        DependencyProperty.Register(
            nameof(ActionSelected),
            typeof(string),
            typeof(MappingControl),
            new PropertyMetadata(null));

    public string ActionSelected
    {
        get => (string)GetValue(ActionSelectedProperty);
        set => SetValue(ActionSelectedProperty, value);
    }

    public static readonly DependencyProperty ActionContentProperty =
        DependencyProperty.Register(
            nameof(ActionContent),
            typeof(object),
            typeof(MappingControl),
            new PropertyMetadata(null));

    public object ActionContent
    {
        get => GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }

    public MappingControl()
    {
        InitializeComponent();

        // For testing purposes, populate the combo-boxes with some dummy data
        TriggersAvailable = new[] { "Trigger 1", "Trigger 2", "Trigger 3" };
        ActionsAvailable = new[] { "Action 1", "Action 2", "Action 3" };
    }
}
