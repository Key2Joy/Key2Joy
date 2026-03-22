using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
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
    /// <summary>Width (px) of each label panel.</summary>
    public const int LabelPanelWidth = 160;

    /// <summary>Height (px) of each label row.</summary>
    private const int LabelHeight = 16;

    /// <summary>Radius (px) of the filled dot drawn on each button.</summary>
    private const int DotRadius = 5;

    private static readonly Color DotColor = Color.FromArgb(220, 50, 50);
    private static readonly Color LineColor = Color.FromArgb(220, 50, 50);
    private static readonly Pen LinePen = new(LineColor, 1.5f) { DashStyle = DashStyle.Dot };

    // Cycle through these suffixes so each label gets a different text length,
    // proving that between 1-4 labels per side all fit without overlap.
    private static readonly string[] TestSuffixes = {
        "Action A",
        "Hold",
        "Tap + Shift",
        "Long Press"
    };

    private readonly Panel _leftPanel;
    private readonly Panel _rightPanel;

    private ControllerDiagramDefinition _definition;
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

    private static Panel CreateSidePanel(bool dockLeft) => new Panel
    {
        Width = LabelPanelWidth,
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
    /// absolutely-positioned label controls inside both side panels.
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
                rightPanelLeft: this._rightPanel.Left);

            this.SuspendLayout();
            this._leftPanel.SuspendLayout();
            this._rightPanel.SuspendLayout();

            this._leftPanel.Controls.Clear();
            this._rightPanel.Controls.Clear();

            var si = 0;
            foreach (var wire in this._wires)
            {
                var panel = wire.IsRight ? this._rightPanel : this._leftPanel;
                var text = wire.Button.Name + ": " + TestSuffixes[si % TestSuffixes.Length];
                si++;

                var labelY = (int)wire.PanelConnector.Y - LabelHeight / 2;

                var lbl = new Label
                {
                    Text = text,
                    AutoSize = false,
                    Left = 2,
                    Top = labelY,
                    Height = LabelHeight,
                    Width = panel.Width - 4,
                    TextAlign = wire.IsRight
                        ? ContentAlignment.MiddleLeft
                        : ContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 7.5f),
                    ForeColor = Color.FromArgb(30, 30, 30),
                    BackColor = Color.Transparent,
                };

                panel.Controls.Add(lbl);
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
        var x = canvas.X + (canvas.Width - w) / 2;
        var y = canvas.Y + (canvas.Height - h) / 2;

        return new Rectangle(x, y, w, h);
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
