using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Key2Joy.Gui.Diagram;

/// <summary>
/// Computes overlap-free wire routes for every button in a
/// <see cref="ControllerDiagramDefinition"/>.
///
/// Routing rules
/// 1. Each button is assigned to the LEFT or RIGHT side based on whether its
///    dot centre is in the left or right half of the controller image rect.
/// 2. Buttons on the same side are sorted by their dot's Y coordinate and
///    assigned evenly-spaced connector Y positions within the usable panel
///    height so that no two horizontal segments are at the same Y.
/// 3. Each wire has two segments:
///      • Diagonal  : dot → elbow  (the elbow is pushed <see cref="DiagonalReach"/>
///                    px horizontally away from the image edge)
///      • Horizontal: elbow → panel connector  (same Y as elbow)
/// </summary>
internal static class WireRouter
{
    /// <summary>
    /// How far (px) the elbow is placed horizontally outside the image rectangle.
    /// A larger value gives the diagonal more room and reduces crossing risk.
    /// </summary>
    public const int DiagonalReach = 80;

    /// <summary>
    /// Vertical margin kept clear at the top and bottom of the label panel
    /// before the first / after the last connector Y.
    /// </summary>
    public const int PanelVerticalMargin = 20;

    /// <summary>
    /// Builds a <see cref="ButtonWire"/> for every button in
    /// <paramref name="definition"/>, given the current layout geometry.
    /// </summary>
    /// <param name="definition">The controller definition (image + buttons).</param>
    /// <param name="imgDest">The destination rectangle of the controller image inside the control.</param>
    /// <param name="controlHeight">Total height of the <see cref="MappingDiagramControl"/>.</param>
    /// <param name="leftPanelRight">X coordinate of the right edge of the left label panel.</param>
    /// <param name="rightPanelLeft">X coordinate of the left edge of the right label panel.</param>
    public static IReadOnlyList<ButtonWire> Build(
        ControllerDiagramDefinition definition,
        Rectangle imgDest,
        int controlHeight,
        int leftPanelRight,
        int rightPanelLeft)
    {
        var img = definition.ControllerImage;
        var scaleX = (float)imgDest.Width / img.Width;
        var scaleY = (float)imgDest.Height / img.Height;

        // 1. Compute dot centres in control coordinates.
        var dotCentres = definition.Buttons
            .Select(b => new PointF(
                imgDest.Left + b.ImagePosition.X * scaleX,
                imgDest.Top + b.ImagePosition.Y * scaleY))
            .ToList();

        var midX = imgDest.Left + imgDest.Width * 0.5f;

        // 2. Partition into left / right groups and sort each by dot Y.
        var left = definition.Buttons
            .Zip(dotCentres, (b, p) => (button: b, dot: p))
            .Where(t => t.dot.X <= midX)
            .OrderBy(t => t.dot.Y)
            .ToList();

        var right = definition.Buttons
            .Zip(dotCentres, (b, p) => (button: b, dot: p))
            .Where(t => t.dot.X > midX)
            .OrderBy(t => t.dot.Y)
            .ToList();

        // 3. Assign evenly-spaced connector Y values for each side.
        var leftConnectorYs = SpreadYs(left.Count, controlHeight, PanelVerticalMargin);
        var rightConnectorYs = SpreadYs(right.Count, controlHeight, PanelVerticalMargin);

        var wires = new List<ButtonWire>(definition.Buttons.Count);

        // 4. Left-side wires.
        for (var i = 0; i < left.Count; i++)
        {
            var (btn, dot) = left[i];
            var connectorY = leftConnectorYs[i];

            // Elbow: same Y as the panel connector, pushed DiagonalReach px
            // to the left of the image edge, but never past the panel inner edge
            // (which would reverse the horizontal segment direction).
            var elbowX = Math.Max(leftPanelRight, imgDest.Left - DiagonalReach);
            var elbow = new PointF(elbowX, connectorY);
            var panelConnector = new PointF(leftPanelRight, connectorY);

            wires.Add(new ButtonWire(btn, dot, elbow, panelConnector, isRight: false));
        }

        // 5. Right-side wires.
        for (var i = 0; i < right.Count; i++)
        {
            var (btn, dot) = right[i];
            var connectorY = rightConnectorYs[i];

            // Elbow: pushed DiagonalReach px to the right of the image edge,
            // but never past the panel inner edge.
            var elbowX = Math.Min(rightPanelLeft, imgDest.Right + DiagonalReach);
            var elbow = new PointF(elbowX, connectorY);
            var panelConnector = new PointF(rightPanelLeft, connectorY);

            wires.Add(new ButtonWire(btn, dot, elbow, panelConnector, isRight: true));
        }

        return wires;
    }

    private static float[] SpreadYs(int count, int totalHeight, int margin)
    {
        if (count == 0)
        {
            return [];
        }

        var ys = new float[count];
        float usable = totalHeight - margin * 2;

        for (var i = 0; i < count; i++)
        {
            // Evenly distribute: first item at margin, last at (totalHeight - margin).
            var t = count == 1 ? 0.5f : (float)i / (count - 1);
            ys[i] = margin + (t * usable);
        }

        return ys;
    }
}
