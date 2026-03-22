using System.Collections.Generic;
using System.Drawing;

namespace Key2Joy.Gui.Diagram;

/// <summary>
/// Combines a controller image with the list of buttons that are present on it.
/// Pass an instance of this class to <see cref="MappingDiagramControl"/> to drive
/// what is rendered.
/// </summary>
public class GamePadDiagramDefinition
{
    /// <summary>The GamePad artwork. Pixel coordinates in <see cref="Buttons"/> are relative to this image.</summary>
    public Image GamePadImage { get; }

    /// <summary>All buttons that should be annotated on the diagram.</summary>
    public IReadOnlyList<GamePadButtonDefinition> Buttons { get; }

    public GamePadDiagramDefinition(Image gamePadImage, IReadOnlyList<GamePadButtonDefinition> buttons)
    {
        this.GamePadImage = gamePadImage;
        this.Buttons = buttons;
    }
}
