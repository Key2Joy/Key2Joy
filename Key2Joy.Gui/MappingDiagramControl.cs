using System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Gui.Diagram;

namespace Key2Joy.Gui;

/// <summary>
/// Renders a controller diagram.
///
/// Layout (left to right):
///   [ left label panel ] [ controller image ] [ right label panel ]
///
/// Each button gets:
///   - A filled dot drawn on the image at its mapped position.
///   - A two-segment wire: diagonal from the dot to a vertical "fence" line
///     outside the image edge, then horizontal from that fence to the panel edge.
///   - An absolutely-positioned Label inside the appropriate side panel whose
///     vertical centre aligns exactly with the wire's horizontal segment.
/// </summary>
public partial class MappingDiagramControl : UserControl
{
    /// <summary>Minimum width (px) of each label panel when no mappings are loaded.</summary>
    public const int MinLabelPanelWidth = 80;

    /// <summary>Horizontal padding (px) added on each side of the widest measured label.</summary>
    private const int LabelPanelPadding = 8;

    /// <summary>Height (px) of each label row.</summary>
    private const int LabelHeight = 22;

    /// <summary>Font used for the button name (bold) in each label block.</summary>
    private static readonly Font LabelFontBold = new Font("Segoe UI", 7.5f, FontStyle.Bold);

    /// <summary>Font used for trigger lines in each label block.</summary>
    private static readonly Font LabelFont = new Font("Segoe UI", 7.5f);

    /// <summary>Radius (px) of the filled dot drawn on each button.</summary>
    private const int DotRadius = 5;

    private static readonly Color DotColor = Color.FromArgb(220, 50, 50);
    private static readonly Color LineColor = Color.FromArgb(220, 50, 50);
    private static readonly Pen LinePen = new(LineColor, 1.5f) { DashStyle = DashStyle.Dot };

    private readonly Panel _leftPanel;
    private readonly Panel _rightPanel;

    private ControllerDiagramDefinition _definition;
    private IReadOnlyList<AbstractMappedOption> _mappings = [];
    private IReadOnlyList<ButtonWire> _wires = [];
    private bool _rebuilding;

    public MappingDiagramControl()
    {
        this.InitializeComponent();

        this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

        this._leftPanel = CreateSidePanel(dockLeft: true);
        this._rightPanel = CreateSidePanel(dockLeft: false);

        // Right panel must be added before left so DockStyle.Right claims space first.
        this.Controls.Add(this._rightPanel);
        this.Controls.Add(this._leftPanel);
    }

    /// <summary>
    /// Assign a <see cref="ControllerDiagramDefinition"/> to drive what is rendered.
    /// Use <see cref="XboxSeriesXControllerDiagram.Create"/> for the Xbox Series X preset.
    /// </summary>
    public ControllerDiagramDefinition Definition
    {
        get => this._definition;
        set
        {
            this._definition = value;
            this.ScheduleRebuild();
        }
    }

    /// <summary>
    /// The active mappings to annotate on the diagram. Each entry whose action matches
    /// a <see cref="ControllerButtonDefinition.ActionMatcher"/> will have its trigger's
    /// display name shown as the label for that button, indicating which keyboard or
    /// mouse input is mapped to that controller button.
    /// </summary>
    public IReadOnlyList<AbstractMappedOption> Mappings
    {
        get => this._mappings;
        set
        {
            this._mappings = value ?? [];
            this.ScheduleRebuild();
        }
    }

    private static Panel CreateSidePanel(bool dockLeft) => new()
    {
        Width = MinLabelPanelWidth,
        Dock = dockLeft ? DockStyle.Left : DockStyle.Right,
        BackColor = Color.Transparent,
        Padding = Padding.Empty,
    };

    /// <summary>
    /// Posts a deferred rebuild so all pending Win32 layout messages are
    /// processed first, guaranteeing stable panel geometry before we compute wires.
    /// </summary>
    private void ScheduleRebuild()
    {
        if (this.IsHandleCreated)
        {
            this.BeginInvoke(new Action(this.Rebuild));
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        this.ScheduleRebuild();
    }

    /// <summary>
    /// Recomputes all wires via <see cref="WireRouter"/> and rebuilds the
    /// absolutely-positioned label block controls inside both side panels.
    /// Each block shows the button name in bold on the first line, followed by
    /// one line per trigger that fires that controller button.
    /// </summary>
    private void Rebuild()
    {
        if (this._rebuilding || this._definition == null || this.ClientSize.Width <= 0)
        {
            return;
        }

        this._rebuilding = true;
        try
        {
            // 1. Collect trigger labels and compute block height for every button.
            var triggersByButton = this._definition.Buttons
                .Select(this.FindTriggerLabels)
                .ToList();

            var blockHeights = new List<int>(this._definition.Buttons.Count);
            using (var g = this.CreateGraphics())
            {
                for (var i = 0; i < this._definition.Buttons.Count; i++)
                {
                    blockHeights.Add(MeasureBlockHeight(g, triggersByButton[i]));
                }
            }

            // 2. Measure widest line across all buttons to size both panels.
            var requiredWidth = MinLabelPanelWidth;
            using (var g = this.CreateGraphics())
            {
                for (var i = 0; i < this._definition.Buttons.Count; i++)
                {
                    var button = this._definition.Buttons[i];
                    var nameWidth = (int)Math.Ceiling(g.MeasureString(button.Name, LabelFontBold).Width);
                    requiredWidth = Math.Max(requiredWidth, nameWidth + (LabelPanelPadding * 2));

                    foreach (var line in triggersByButton[i])
                    {
                        var lineWidth = (int)Math.Ceiling(g.MeasureString(line, LabelFont).Width);
                        requiredWidth = Math.Max(requiredWidth, lineWidth + (LabelPanelPadding * 2));
                    }
                }
            }

            this.SuspendLayout();
            this._leftPanel.SuspendLayout();
            this._rightPanel.SuspendLayout();

            this._leftPanel.Width = requiredWidth;
            this._rightPanel.Width = requiredWidth;

            this._leftPanel.ResumeLayout(false);
            this._rightPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            // 3. Now that panels have their final widths, compute wire geometry.
            var imgDest = this.GetImageDestRect();
            if (imgDest.IsEmpty)
            {
                return;
            }

            this._wires = WireRouter.Build(
                this._definition,
                imgDest,
                this.ClientSize.Height,
                leftPanelRight: this._leftPanel.Right,
                rightPanelLeft: this._rightPanel.Left,
                blockHeights: blockHeights);

            this.SuspendLayout();
            this._leftPanel.SuspendLayout();
            this._rightPanel.SuspendLayout();

            this._leftPanel.Controls.Clear();
            this._rightPanel.Controls.Clear();

            foreach (var wire in this._wires)
            {
                var panel = wire.IsRight ? this._rightPanel : this._leftPanel;
                var idx = this._definition.Buttons
                    .Select((b, i) => (b, i))
                    .First(x => ReferenceEquals(x.b, wire.Button)).i;
                var triggers = triggersByButton[idx];
                var blockH = blockHeights[idx];
                var blockTop = (int)wire.PanelConnector.Y - (blockH / 2);
                var innerWidth = panel.Width - (LabelPanelPadding * 2);
                var isRight = wire.IsRight;

                // Outer panel that clips and positions the whole block.
                var block = new Panel
                {
                    Left = LabelPanelPadding,
                    Top = blockTop,
                    Width = innerWidth,
                    Height = blockH,
                    BackColor = Color.Transparent,
                    Padding = Padding.Empty,
                };

                // Bold button name on the first line.
                var nameLabel = new Label
                {
                    Text = wire.Button.Name,
                    AutoSize = false,
                    Left = 0,
                    Top = 0,
                    Width = innerWidth,
                    Height = LabelHeight,
                    TextAlign = isRight ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight,
                    Font = LabelFontBold,
                    ForeColor = Color.FromArgb(30, 30, 30),
                    BackColor = Color.Transparent,
                };
                block.Controls.Add(nameLabel);

                // One trigger line per mapping.
                for (var t = 0; t < triggers.Count; t++)
                {
                    var triggerLabel = new Label
                    {
                        Text = triggers[t],
                        AutoSize = false,
                        Left = 0,
                        Top = LabelHeight + (t * LabelHeight),
                        Width = innerWidth,
                        Height = LabelHeight,
                        TextAlign = isRight ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight,
                        Font = LabelFont,
                        ForeColor = Color.FromArgb(80, 80, 80),
                        BackColor = Color.Transparent,
                    };
                    block.Controls.Add(triggerLabel);
                }

                panel.Controls.Add(block);
            }

            this._leftPanel.ResumeLayout(false);
            this._rightPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        finally
        {
            this._rebuilding = false;
        }

        this.Invalidate();
    }

    /// <summary>
    /// Returns the total pixel height of a label block: one bold name row plus
    /// one row per trigger line (or one "Unbound" row when there are none).
    /// </summary>
    private static int MeasureBlockHeight(Graphics g, IReadOnlyList<string> triggerLines)
        => LabelHeight * (1 + Math.Max(1, triggerLines.Count));

    /// <summary>
    /// Destination rectangle for the controller image: centred in the space
    /// between the two side panels with aspect ratio preserved.
    /// </summary>
    private Rectangle GetImageDestRect()
    {
        if (this._definition?.ControllerImage == null)
        {
            return Rectangle.Empty;
        }

        var img = this._definition.ControllerImage;
        var canvas = new Rectangle(
            this._leftPanel.Right,
            0,
            this._rightPanel.Left - this._leftPanel.Right,
            this.ClientSize.Height);

        if (canvas.Width <= 0 || canvas.Height <= 0)
        {
            return Rectangle.Empty;
        }

        var scaleX = (float)canvas.Width / img.Width;
        var scaleY = (float)canvas.Height / img.Height;
        var scale = Math.Min(scaleX, scaleY);

        var w = (int)(img.Width * scale);
        var h = (int)(img.Height * scale);
        var x = canvas.X + ((canvas.Width - w) / 2);
        var y = canvas.Y + ((canvas.Height - h) / 2);

        return new Rectangle(x, y, w, h);
    }

    /// <summary>
    /// Returns the trigger display name for every mapping whose action targets
    /// <paramref name="button"/>. Returns an empty list when there is no match
    /// or the button has no <see cref="ControllerButtonDefinition.ActionMatcher"/>.
    /// </summary>
    private IReadOnlyList<string> FindTriggerLabels(ControllerButtonDefinition button)
    {
        if (button.ActionMatcher == null || this._mappings == null)
        {
            return [];
        }

        return this._mappings
            .Where(m => m.Action != null && button.ActionMatcher(m.Action))
            .Select(m => m.Trigger?.GetNameDisplay() ?? string.Empty)
            .Where(s => s.Length > 0)
            .ToList();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        if (this._definition?.ControllerImage == null)
        {
            return;
        }

        var imgDest = this.GetImageDestRect();
        if (imgDest.IsEmpty)
        {
            return;
        }

        // 1. Controller image.
        g.DrawImage(this._definition.ControllerImage, imgDest);

        if (this._wires == null || this._wires.Count == 0)
        {
            return;
        }

        // 2. Wires and dots.
        using (var dotBrush = new SolidBrush(DotColor))
        {
            foreach (var wire in this._wires)
            {
                // Segment 1 – diagonal: dot centre --> elbow (fence point).
                g.DrawLine(LinePen, wire.DotCenter, wire.Elbow);

                // Segment 2 – horizontal: elbow --> panel connector.
                g.DrawLine(LinePen, wire.Elbow, wire.PanelConnector);

                // Dot.
                g.FillEllipse(
                    dotBrush,
                    wire.DotCenter.X - DotRadius,
                    wire.DotCenter.Y - DotRadius,
                    DotRadius * 2,
                    DotRadius * 2);
            }
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        this.ScheduleRebuild();
    }
}
