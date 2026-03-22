using System.Collections.Generic;
using Key2Joy.Gui.Properties;

namespace Key2Joy.Gui.Diagram;

/// <summary>
/// Pre-configured <see cref="ControllerDiagramDefinition"/> for the Xbox Series X controller.
/// All button coordinates are pixel-accurate for the 1452 × 940 source image
/// (<c>Resources.xbox_series_x</c>).
/// </summary>
public static class XboxSeriesXControllerDiagram
{
    // ---------------------------------------------------------------------------
    // Image dimensions of Resources.xbox_series_x: 1452 × 940
    //
    // Coordinate reference:
    //   Origin (0,0) = top-left corner of the image.
    //   All values were measured on the unscaled 1452 × 940 PNG.
    // ---------------------------------------------------------------------------

    private static readonly IReadOnlyList<ControllerButtonDefinition> Buttons =
        [
            // Face buttons (right cluster)
            new("A",      1088, 494),
            new("B",      1188, 400),
            new("X",      996, 405),
            new("Y",      1092, 318),

            // Bumpers / Triggers
            new("LB",      440, 150),
            new("RB",     1020, 150),
            new("LT",      360, 75),
            new("RT",     1100, 75),

            // D-Pad
            new("DPad Up",    550, 550),
            new("DPad Down",  550, 675),
            new("DPad Left",  480, 620),
            new("DPad Right", 620, 620),

            // Analog sticks
            new("Left Stick",  375, 445),
            new("Right Stick", 915, 650),

            // Center buttons
            new("View",   630, 410),
            new("Menu",   830, 410),
            // new("Xbox",   730, 315),
            // new("Share",  730, 435),
        ];

    /// <summary>
    /// Creates a new <see cref="ControllerDiagramDefinition"/> backed by the Xbox Series X
    /// artwork and button layout.
    /// </summary>
    public static ControllerDiagramDefinition Create()
        => new(Resources.xbox_series_x, Buttons);
}
