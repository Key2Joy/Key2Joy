using System.Collections.Generic;
using System.Drawing;

namespace Key2Joy.Gui.Diagram;

/// <summary>
/// Combines a controller image with the list of buttons that are present on it.
/// Pass an instance of this class to <see cref="MappingDiagramControl"/> to drive
/// what is rendered.
/// </summary>
public class ControllerDiagramDefinition
{
    /// <summary>The controller artwork. Pixel coordinates in <see cref="Buttons"/> are relative to this image.</summary>
    public Image ControllerImage { get; }

    /// <summary>All buttons that should be annotated on the diagram.</summary>
    public IReadOnlyList<ControllerButtonDefinition> Buttons { get; }

    public ControllerDiagramDefinition(Image controllerImage, IReadOnlyList<ControllerButtonDefinition> buttons)
    {
        this.ControllerImage = controllerImage;
        this.Buttons = buttons;
    }
}
