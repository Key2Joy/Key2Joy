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
///    assigned connector Y positions at the vertical midpoint of each button's
///    label block, spaced proportionally so that the total block height fills
///    the usable panel height evenly.
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
    /// Each button's wire connector is placed at the vertical midpoint of its
    /// label block so the horizontal wire segment aligns with the block centre.
    /// </summary>
    /// <param name="definition">The controller definition (image + buttons).</param>
    /// <param name="imgDest">The destination rectangle of the controller image inside the control.</param>
    /// <param name="controlHeight">Total height of the <see cref="MappingDiagramControl"/>.</param>
    /// <param name="leftPanelRight">X coordinate of the right edge of the left label panel.</param>
    /// <param name="rightPanelLeft">X coordinate of the left edge of the right label panel.</param>
    /// <param name="blockHeights">
    /// Per-button block heights in the same order as <paramref name="definition"/>.Buttons.
    /// When <see langword="null"/> all blocks are treated as equal height.
    /// </param>
    public static IReadOnlyList<ButtonWire> Build(
        ControllerDiagramDefinition definition,
        Rectangle imgDest,
        int controlHeight,
        int leftPanelRight,
        int rightPanelLeft,
        IReadOnlyList<int> blockHeights = null)
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
        //    Carry the original button index so we can look up block heights.
        var left = definition.Buttons
            .Select((b, idx) => (button: b, dot: dotCentres[idx], idx))
            .Where(t => t.dot.X <= midX)
            .OrderBy(t => t.dot.Y)
            .ToList();

        var right = definition.Buttons
            .Select((b, idx) => (button: b, dot: dotCentres[idx], idx))
            .Where(t => t.dot.X > midX)
            .OrderBy(t => t.dot.Y)
            .ToList();

        // 3. Assign connector Y values at each block's vertical midpoint.
        var leftHeights = left.Select(t => blockHeights != null ? blockHeights[t.idx] : 1).ToList();
        var rightHeights = right.Select(t => blockHeights != null ? blockHeights[t.idx] : 1).ToList();

        var leftConnectorYs = MidpointYs(leftHeights, controlHeight, PanelVerticalMargin);
        var rightConnectorYs = MidpointYs(rightHeights, controlHeight, PanelVerticalMargin);

        var wires = new List<ButtonWire>(definition.Buttons.Count);

        // 4. Left-side wires.
        for (var i = 0; i < left.Count; i++)
        {
            var (btn, dot, _) = left[i];
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
            var (btn, dot, _) = right[i];
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

    /// <summary>
    /// Returns the Y coordinate of the vertical midpoint of each block, distributed
    /// so that the blocks (separated by equal gaps) fill the usable height.
    /// </summary>
    private static float[] MidpointYs(IList<int> heights, int totalHeight, int margin)
    {
        if (heights.Count == 0)
        {
            return [];
        }

        var ys = new float[heights.Count];
        float usable = totalHeight - margin * 2;
        var totalBlockHeight = heights.Sum();

        // Gap between consecutive blocks (evenly distributed remaining space).
        var gap = heights.Count > 1
            ? (usable - totalBlockHeight) / (heights.Count - 1)
            : 0f;

        // Clamp gap to zero so blocks never overlap even if they overflow.
        if (gap < 0)
        {
            gap = 0f;
        }

        float y = margin;
        for (var i = 0; i < heights.Count; i++)
        {
            ys[i] = y + heights[i] / 2f;
            y += heights[i] + gap;
        }

        return ys;
    }
}
