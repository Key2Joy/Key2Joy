using System;
using System;
using System.Drawing;
using Key2Joy.Contracts.Mapping.Actions;

namespace Key2Joy.Gui.Diagram;

/// <summary>
/// Represents a single named button on a GamePad image, identified by its
/// pixel position relative to the top-left corner of the source image.
/// </summary>
public class GamePadButtonDefinition
{
    /// <summary>Display name for this button (e.g. "A", "LB", "Left Stick").</summary>
    public string Name { get; }

    /// <summary>
    /// Position of the button's centre in pixels, relative to the top-left of
    /// the <em>original</em> (unscaled) GamePad image.
    /// </summary>
    public Point ImagePosition { get; }

    /// <summary>
    /// Optional predicate that returns <see langword="true"/> when the given action
    /// targets this GamePad button. Used to match <see cref="AbstractAction"/>
    /// instances from a <see cref="Key2Joy.Mapping.MappingProfile"/> to this button,
    /// so that the trigger's display name can be shown as the label.
    /// </summary>
    public Func<AbstractAction, bool> ActionMatcher { get; }

    public GamePadButtonDefinition(string name, Point imagePosition, Func<AbstractAction, bool> actionMatcher = null)
    {
        this.Name = name;
        this.ImagePosition = imagePosition;
        this.ActionMatcher = actionMatcher;
    }

    public GamePadButtonDefinition(string name, int x, int y, Func<AbstractAction, bool> actionMatcher = null)
        : this(name, new Point(x, y), actionMatcher) { }
}
