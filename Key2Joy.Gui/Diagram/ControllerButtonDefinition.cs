using System.Drawing;

namespace Key2Joy.Gui.Diagram;

/// <summary>
/// Represents a single named button on a controller image, identified by its
/// pixel position relative to the top-left corner of the source image.
/// </summary>
public class ControllerButtonDefinition
{
    /// <summary>Display name for this button (e.g. "A", "LB", "Left Stick").</summary>
    public string Name { get; }

    /// <summary>
    /// Position of the button's centre in pixels, relative to the top-left of
    /// the <em>original</em> (unscaled) controller image.
    /// </summary>
    public Point ImagePosition { get; }

    public ControllerButtonDefinition(string name, Point imagePosition)
    {
        this.Name = name;
        this.ImagePosition = imagePosition;
    }

    public ControllerButtonDefinition(string name, int x, int y)
        : this(name, new Point(x, y)) { }
}
