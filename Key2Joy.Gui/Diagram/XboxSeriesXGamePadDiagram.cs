using System.Collections.Generic;
using Key2Joy.Gui.Properties;
using Key2Joy.Mapping.Actions.Input;
using Key2Joy.Mapping.Triggers.GamePad;
using SimWinInput;

namespace Key2Joy.Gui.Diagram;

/// <summary>
/// Pre-configured <see cref="GamePadDiagramDefinition"/> for the Xbox Series X GamePad.
/// All button coordinates are pixel-accurate for the 1452 × 940 source image
/// (<c>Resources.xbox_series_x</c>).
/// </summary>
public static class XboxSeriesXGamePadDiagram
{
    // ---------------------------------------------------------------------------
    // Image dimensions of Resources.xbox_series_x: 1452 × 940
    //
    // Coordinate reference:
    //   Origin (0,0) = top-left corner of the image.
    //   All values were measured on the unscaled 1452 × 940 PNG.
    // ---------------------------------------------------------------------------

    private static readonly IReadOnlyList<GamePadButtonDefinition> Buttons =
        [
            // Face buttons (right cluster)
            new("A",      1088, 494, a => a is GamePadButtonAction b && b.Control == GamePadControl.A),
            new("B",      1188, 400, a => a is GamePadButtonAction b && b.Control == GamePadControl.B),
            new("X",      996,  405, a => a is GamePadButtonAction b && b.Control == GamePadControl.X),
            new("Y",      1092, 318, a => a is GamePadButtonAction b && b.Control == GamePadControl.Y),

            // Bumpers / Triggers
            new("LB",  440, 150, a => a is GamePadButtonAction b && b.Control == GamePadControl.LeftShoulder),
            new("RB", 1020, 150, a => a is GamePadButtonAction b && b.Control == GamePadControl.RightShoulder),
            new("LT",  360,  75, a => a is GamePadTriggerAction t && t.Side == GamePadSide.Left),
            new("RT", 1100,  75, a => a is GamePadTriggerAction t && t.Side == GamePadSide.Right),

            // D-Pad
            new("DPad Up",    550, 550, a => a is GamePadButtonAction b && b.Control == GamePadControl.DPadUp),
            new("DPad Down",  550, 675, a => a is GamePadButtonAction b && b.Control == GamePadControl.DPadDown),
            new("DPad Left",  480, 620, a => a is GamePadButtonAction b && b.Control == GamePadControl.DPadLeft),
            new("DPad Right", 620, 620, a => a is GamePadButtonAction b && b.Control == GamePadControl.DPadRight),

            // Analog sticks
            new("Left Stick",  375, 445, a => a is GamePadStickAction s && s.Side == GamePadSide.Left),
            new("Right Stick", 915, 650, a => a is GamePadStickAction s && s.Side == GamePadSide.Right),

            // Center buttons
            new("View",  630, 410, a => a is GamePadButtonAction b && b.Control == GamePadControl.Back),
            new("Menu",  830, 410, a => a is GamePadButtonAction b && b.Control == GamePadControl.Start),
            // new("Xbox",  730, 315),
            // new("Share", 730, 435),
        ];

    /// <summary>
    /// Creates a new <see cref="GamePadDiagramDefinition"/> backed by the Xbox Series X
    /// artwork and button layout.
    /// </summary>
    public static GamePadDiagramDefinition Create()
        => new(Resources.xbox_series_x, Buttons);
}
