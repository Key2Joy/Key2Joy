using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using DependencyPropertyGenerator;

namespace Key2Joy.App.UserControls;

[DependencyProperty<bool>("IsOpen")]
[DependencyProperty<object>("DrawerContent")]
[DependencyProperty<double>("DrawerHeight")]
[DependencyProperty<bool>("CloseOnOverlayTap")]
public sealed partial class DrawerControl : UserControl
{
    public event EventHandler DrawerOpened;
    public event EventHandler DrawerClosed;

    private static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(300);

    public DrawerControl()
    {
        this.InitializeComponent();

        this.Loaded += this.DrawerControl_Loaded;
    }

    private void DrawerControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.DrawerPanel.Height = this.DrawerHeight;

        // Start hidden off-screen
        this.DrawerTranslate.Y = this.DrawerHeight;
        this.Overlay.Opacity = 0;
        this.Overlay.IsHitTestVisible = false;
    }

    partial void OnIsOpenChanged(bool newValue)
    {
        if (newValue)
        {
            this.Open();
        }
        else
        {
            this.Close();
        }
    }

    partial void OnDrawerHeightChanged(double newValue)
    {
        this.DrawerPanel.Height = newValue;

        if (!this.IsOpen)
        {
            this.DrawerTranslate.Y = newValue;
        }
    }

    private void Open()
    {
        this.DrawerPanel.Height = this.DrawerHeight;

        this.AnimateTranslateY(this.DrawerTranslate.Y, 0);
        this.AnimateOverlay(this.Overlay.Opacity, 1);
        this.Overlay.IsHitTestVisible = true;

        DrawerOpened?.Invoke(this, EventArgs.Empty);
    }

    private void Close()
    {
        this.AnimateTranslateY(this.DrawerTranslate.Y, this.DrawerHeight);
        this.AnimateOverlay(this.Overlay.Opacity, 0);
        this.Overlay.IsHitTestVisible = false;

        DrawerClosed?.Invoke(this, EventArgs.Empty);
    }

    private void AnimateTranslateY(double from, double to)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = new Duration(AnimationDuration),
            EasingFunction = new CircleEase
            {
                EasingMode = EasingMode.EaseOut
            },
            EnableDependentAnimation = true,
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        Storyboard.SetTarget(animation, this.DrawerTranslate);
        Storyboard.SetTargetProperty(animation, "Y");
        storyboard.Begin();
    }

    private void AnimateOverlay(double from, double to)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = new Duration(AnimationDuration),
            EasingFunction = new CircleEase
            {
                EasingMode = EasingMode.EaseOut
            },
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        Storyboard.SetTarget(animation, this.Overlay);
        Storyboard.SetTargetProperty(animation, "Opacity");
        storyboard.Begin();
    }

    private void Overlay_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (this.CloseOnOverlayTap)
        {
            this.IsOpen = false;
        }
    }

    private void GripBar_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        // Toggle drawer on grip bar click
        this.IsOpen = !this.IsOpen;
    }
}
