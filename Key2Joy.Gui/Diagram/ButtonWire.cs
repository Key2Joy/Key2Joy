using System.Drawing;

namespace Key2Joy.Gui.Diagram;

/// <summary>
/// Describes the fully-resolved routing for one button's connector line.
/// Computed by <see cref="WireRouter"/> and consumed by the painter.
/// </summary>
internal class ButtonWire
{
    /// <summary>The button this wire belongs to.</summary>
    public ControllerButtonDefinition Button { get; }

    /// <summary>Centre of the dot drawn on the controller image (control-client coords).</summary>
    public PointF DotCenter { get; }

    /// <summary>
    /// The "elbow" – first waypoint outside the image edge.
    /// The line goes <see cref="DotCenter"/> → <see cref="Elbow"/> diagonally,
    /// then <see cref="Elbow"/> → <see cref="PanelConnector"/> horizontally.
    /// </summary>
    public PointF Elbow { get; }

    /// <summary>The point on the panel edge where the horizontal segment terminates.</summary>
    public PointF PanelConnector { get; }

    /// <summary>True = wire exits to the right panel; false = left panel.</summary>
    public bool IsRight { get; }

    public ButtonWire(
        ControllerButtonDefinition button,
        PointF dotCenter,
        PointF elbow,
        PointF panelConnector,
        bool isRight)
    {
        this.Button = button;
        this.DotCenter = dotCenter;
        this.Elbow = elbow;
        this.PanelConnector = panelConnector;
        this.IsRight = isRight;
    }
}
