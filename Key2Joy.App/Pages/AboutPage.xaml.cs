using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace Key2Joy.App.Pages;

public sealed partial class AboutPage : Page
{
    private record CreditEntry(string ProjectName, string Description, string Author, string Url);

    private static readonly List<CreditEntry> Credits =
    [
        new(
            "SimWinInput",
            "Allows simulating the controller.",
            "David Rieman",
            "https://github.com/DavidRieman/SimWinInput"
        ),

        new(
            "ScpVBus",
            "Driver that makes SimWinInput possible.",
            "Benjamin Höglinger-Stelzer",
            "https://github.com/nefarius/ScpVBus"
        ),

        new(
            "JoyToKey",
            "Inspiration — allows simulating keyboard and mouse with a joystick.",
            "JoyToKey",
            "https://joytokey.net/en/"
        ),

        new(
            "Silk Icons",
            "A great icon set used throughout the application.",
            "Mark James",
            "https://github.com/legacy-icons/famfamfam-silk"
        ),
    ];

    public AboutPage()
    {
        this.InitializeComponent();

        this.ProductNameTextBlock.Text = AssemblyProduct;
        this.VersionTextBlock.Text = $"Version {Version}";
        this.CopyrightTextBlock.Text = AssemblyCopyright;

        this.BuildCreditCards();
    }

    private void BuildCreditCards()
    {
        foreach (var credit in Credits)
        {
            var card = CreateCreditCard(credit);
            this.CreditsItemsControl.Items.Add(card);
        }
    }

    private static UIElement CreateCreditCard(CreditEntry credit)
    {
        // Outer border styled as a WinUI card
        var border = new Border
        {
            Background = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
            BorderBrush = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
        };

        var grid = new Grid
        {
            ColumnSpacing = 12,
        };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Left side: project info
        var infoPanel = new StackPanel { Spacing = 2 };

        var titleText = new TextBlock
        {
            Text = credit.ProjectName,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 14,
        };
        infoPanel.Children.Add(titleText);

        var descText = new TextBlock
        {
            Text = credit.Description,
            TextWrapping = TextWrapping.Wrap,
            Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"],
            FontSize = 13,
        };
        infoPanel.Children.Add(descText);

        var authorText = new TextBlock
        {
            Text = $"by {credit.Author}",
            Foreground = (Brush)Application.Current.Resources["TextFillColorTertiaryBrush"],
            FontSize = 12,
            Margin = new Thickness(0, 4, 0, 0),
        };
        infoPanel.Children.Add(authorText);

        Grid.SetColumn(infoPanel, 0);
        grid.Children.Add(infoPanel);

        // Right side: link button
        var linkButton = new HyperlinkButton
        {
            Content = "View \u2197",
            NavigateUri = new Uri(credit.Url),
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Thickness(12, 6, 12, 6),
        };

        Grid.SetColumn(linkButton, 1);
        grid.Children.Add(linkButton);

        border.Child = grid;
        return border;
    }

    private static T? GetAttributeValue<T>(Func<Assembly, object[]> getAttributesFunc) where T : Attribute
    {
        var attributes = getAttributesFunc(Assembly.GetExecutingAssembly());
        return attributes.Length > 0 ? attributes[0] as T : null;
    }

    public static string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

    public static string AssemblyProduct
        => GetAttributeValue<AssemblyProductAttribute>(
                asm => asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false)
            )?.Product ?? string.Empty;

    public static string AssemblyCopyright => GetAttributeValue<AssemblyCopyrightAttribute>(
                    asm => asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)
                )?.Copyright ?? string.Empty;
}
