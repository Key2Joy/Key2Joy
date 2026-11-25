using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping.Triggers.GamePad;
using Key2Joy.Mapping.Triggers.Mouse;
using Key2Joy.Util;

namespace Key2Joy.Mapping.Actions.Input;

[Action(
    Description = "GamePad Stick Simulation",
    NameFormat = "Move {0} Stick on GamePad #{3} by ({1}, {2})",
    GroupName = "GamePad Simulation"
)]
public class GamePadStickAction : CoreAction, IProvideReverseAspect, IEquatable<GamePadStickAction>
{
    /// <summary>
    /// This describes what 'a lot of input movement' is, so we can scale
    /// the delta and make the scaling numbers feel intuitive.
    /// </summary>
    private const int BIG_DELTA_MOVEMENT = 250;

    private const short MIN_STICK_VALUE = XInputGamePad.ThumbstickValueMin;
    private const short MAX_STICK_VALUE = XInputGamePad.ThumbstickValueMax;

    /// <summary>
    /// Which side to simulate
    /// </summary>
    public GamePadSide Side { get; set; }

    /// <summary>
    /// How much to move the stick on the X axis, as a fraction of full deflection
    /// (-1 is fully left, 1 is fully right). If null, then the inputbag of the input
    /// that triggered this will be used if possible
    /// </summary>
    public float? DeltaX { get; set; } = null;

    /// <summary>
    /// How much to move the stick on the Y axis, as a fraction of full deflection
    /// (-1 is fully down, 1 is fully up). If null, then the inputbag of the input
    /// that triggered this will be used if possible
    /// </summary>
    public float? DeltaY { get; set; } = null;

    /// <summary>
    /// If DeltaX is null, this is the scale factor to apply to the inputbag
    /// </summary>
    public float InputScaleX { get; set; } = 1;

    /// <summary>
    /// If DeltaY is null, this is the scale factor to apply to the inputbag
    /// </summary>
    public float InputScaleY { get; set; } = -1;

    /// <summary>
    /// After how many milliseconds without this action executing should what it
    /// moved be undone? 0 or less means never.
    /// </summary>
    public int ResetAfterIdleTimeInMs { get; set; } = 500;

    /// <summary>
    /// Which gamepad to simulate
    /// </summary>
    public int GamePadIndex { get; set; }

    private System.Timers.Timer noInputTimer;

    /// <summary>
    /// Axes this action added relative (exact mode) moves to since the last reset.
    /// </summary>
    private HashSet<(int GamePadIndex, GamePadSide Side, GamePadStickAxis Axis)> movedRelativeAxes;

    /// <summary>
    /// Sticks this action contributed to since the last reset, so they can be updated
    /// when the contributions are undone.
    /// </summary>
    private HashSet<(int GamePadIndex, GamePadSide Side)> touchedSticks;

    public GamePadStickAction(string name) : base(name)
        => this.InitializeResetState();

    private void InitializeResetState()
    {
        this.noInputTimer = new System.Timers.Timer();
        this.noInputTimer.Elapsed += this.NoInputTimer_Elapsed;
        this.noInputTimer.AutoReset = false;
        this.movedRelativeAxes = new();
        this.touchedSticks = new();
    }

    /// <inheritdoc/>
    public override object Clone()
    {
        // A memberwise clone would share the timer (bound to this instance) with the clone
        var clone = (GamePadStickAction)base.Clone();
        clone.InitializeResetState();
        return clone;
    }

    /// <inheritdoc/>
    public void MakeReverse(AbstractMappingAspect aspect)
    {
        var reverse = aspect as GamePadStickAction;

        if (this.DeltaX.HasValue
            && this.DeltaY.HasValue)
        {
            reverse.DeltaX = this.DeltaX.Value * -1;
            reverse.DeltaY = this.DeltaY.Value * -1;
        }
    }

    /// <inheritdoc/>
    public override void OnStartListening(AbstractTriggerListener listener, ref IList<AbstractAction> otherActions)
    {
        base.OnStartListening(listener, ref otherActions);

        var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();
        gamePadService.EnsurePluggedIn(this.GamePadIndex);
    }

    /// <inheritdoc/>
    public override void OnStopListening(AbstractTriggerListener listener)
    {
        base.OnStopListening(listener);

        this.noInputTimer.Stop();
        this.UndoContributions();
    }

    /// <markdown-doc>
    /// <parent-name>Input</parent-name>
    /// <path>Api/Input</path>
    /// </markdown-doc>
    /// <summary>
    /// Simulate moving a gamepad joystick
    /// </summary>
    /// <markdown-example>
    /// Moves the left gamepad joystick halfway up and to the right, then resets after 500ms
    /// <code language="lua">
    /// <![CDATA[
    /// GamePad.SimulateMove(0.5,0.5)
    /// SetTimeout(function()
    ///    GamePad.Reset()
    /// end, 500)
    /// ]]>
    /// </code>
    /// </markdown-example>
    /// <param name="deltaX">The fraction (-1 to 1) by which to move the stick right (positive) or left (negative)</param>
    /// <param name="deltaY">The fraction (-1 to 1) by which to move the stick up (positive) or down (negative)</param>
    /// <param name="side">Which gamepad stick to move, either GamePadSide.Left (default) or .Right</param>
    /// <param name="gamepadIndex">Which of 4 possible gamepads to simulate: 0 (default), 1, 2 or 3</param>
    /// <name>GamePad.SimulateMove</name>
    [ExposesScriptingMethod("GamePad.SimulateMove")]
    public async void ExecuteForScript(
        float deltaX,
        float deltaY,
        GamePadSide side = GamePadSide.Left,
        int gamepadIndex = 0)
    {
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
        this.Side = side;
        this.GamePadIndex = gamepadIndex;

        var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();
        gamePadService.EnsurePluggedIn(this.GamePadIndex);

        await this.Execute();
    }

    /// <summary>
    /// Scales InputBag values to a reasonable range for gamepad sticks
    /// </summary>
    /// <param name="value"></param>
    /// <param name="sensitivity"></param>
    /// <returns></returns>
    private static short Scale(int value, float sensitivity = 1.0f)
    {
        var scaled = (int)((float)value / BIG_DELTA_MOVEMENT * sensitivity * (MAX_STICK_VALUE - MIN_STICK_VALUE));
        var gamePadStickX = (short)Math.Min(Math.Max(scaled, MIN_STICK_VALUE), MAX_STICK_VALUE);

        return gamePadStickX;
    }

    /// <summary>
    /// Converts a fraction of full deflection (-1 to 1) to a gamepad stick value
    /// </summary>
    /// <param name="fraction"></param>
    /// <returns></returns>
    private static short FractionToStickValue(float fraction)
    {
        var clamped = Math.Min(Math.Max(fraction, -1f), 1f);

        return (short)Math.Round(clamped * MAX_STICK_VALUE);
    }

    /// <inheritdoc/>
    public override async Task Execute(AbstractInputBag inputBag = null)
    {
        var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();
        var gamePad = gamePadService.GetGamePad(this.GamePadIndex);

        if (!gamePad.GetIsPluggedIn())
        {
            gamePad.PlugIn();
        }

        var movedX = this.ContributeToAxis(GamePadStickAxis.X, this.DeltaX, inputBag);
        var movedY = this.ContributeToAxis(GamePadStickAxis.Y, this.DeltaY, inputBag);

        if ((movedX || movedY) && this.ResetAfterIdleTimeInMs > 0)
        {
            // Restart the timer, since there was input
            this.noInputTimer.Stop();
            this.noInputTimer.Interval = this.ResetAfterIdleTimeInMs;
            this.noInputTimer.Start();
        }

        GamePadStickMixer.Apply(gamePad, this.Side);
    }

    /// <summary>
    /// Contributes the exact delta, or the value from the input bag, to an axis.
    /// </summary>
    /// <returns>Whether anything was contributed</returns>
    private bool ContributeToAxis(GamePadStickAxis axis, float? exactDelta, AbstractInputBag inputBag)
    {
        var isX = axis == GamePadStickAxis.X;

        if (exactDelta is not null)
        {
            // Exact moves add to a total shared with other exact moves, so a press and its
            // (separate) reverse release always cancel out exactly.
            var delta = FractionToStickValue(exactDelta.Value);

            if (delta == 0)
            {
                return false;
            }

            GamePadStickMixer.AddRelative(this.GamePadIndex, this.Side, axis, delta);

            lock (this.movedRelativeAxes)
            {
                this.movedRelativeAxes.Add((this.GamePadIndex, this.Side, axis));
                this.touchedSticks.Add((this.GamePadIndex, this.Side));
            }

            return true;
        }

        if (inputBag is AxisDeltaInputBag axisInputBag)
        {
            // Relative input (e.g. mouse movement) accumulates in this action's own value,
            // so undoing it doesn't affect what other mappings hold on this axis.
            var delta = isX
                ? Scale(axisInputBag.DeltaX, this.InputScaleX)
                : Scale(axisInputBag.DeltaY, this.InputScaleY);

            if (delta == 0)
            {
                return false;
            }

            GamePadStickMixer.AddToSource(this.GamePadIndex, this.Side, axis, this, delta);
            this.TrackTouchedStick();
            return true;
        }

        if (inputBag is GamePadTriggerInputBag triggerInputBag)
        {
            // TODO: This is now hard-coded, but I'd love for 'modifiers' to exist in between triggers and actions. Those could (with more fine tuning) be configured by the user.
            // For triggers we need to not have a delta, but simply a value that maps directly to the stick value (feels better)
            var value = isX
                ? Scale((int)(triggerInputBag.LeftTriggerDelta * XInputGamePad.TriggerValueMax), this.InputScaleX)
                : Scale((int)(triggerInputBag.RightTriggerDelta * XInputGamePad.TriggerValueMax), this.InputScaleY);

            GamePadStickMixer.SetSource(this.GamePadIndex, this.Side, axis, this, value);
            this.TrackTouchedStick();
            return value != 0;
        }

        return false;
    }

    private void TrackTouchedStick()
    {
        lock (this.movedRelativeAxes)
        {
            this.touchedSticks.Add((this.GamePadIndex, this.Side));
        }
    }

    /// <summary>
    /// Undoes what this action moved if there was no input for a while
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NoInputTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        => this.UndoContributions();

    /// <summary>
    /// Undoes what this action contributed to the sticks:
    /// - Its own values (from input bags) are removed, leaving other mappings alone.
    /// - The shared total of exact moves is reset on the axes this action moved. That
    ///   total is shared with the reverse mapping (e.g. key release), so it can't be
    ///   undone per action. This matches how the reset always behaved.
    /// </summary>
    private void UndoContributions()
    {
        List<(int GamePadIndex, GamePadSide Side, GamePadStickAxis Axis)> relativeAxes;
        List<(int GamePadIndex, GamePadSide Side)> sticks;

        lock (this.movedRelativeAxes)
        {
            relativeAxes = new(this.movedRelativeAxes);
            sticks = new(this.touchedSticks);
            this.movedRelativeAxes.Clear();
            this.touchedSticks.Clear();
        }

        if (sticks.Count == 0)
        {
            return;
        }

        foreach (var (gamePadIndex, side, axis) in relativeAxes)
        {
            GamePadStickMixer.ResetRelative(gamePadIndex, side, axis);
        }

        GamePadStickMixer.RemoveSource(this);

        var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();

        foreach (var (gamePadIndex, side) in sticks)
        {
            var gamePad = gamePadService.GetGamePad(gamePadIndex);

            if (gamePad.GetIsPluggedIn())
            {
                GamePadStickMixer.Apply(gamePad, side);
            }
        }
    }

    /// <inheritdoc/>
    public override string GetNameDisplay()
        => this.Name.Replace("{0}", Enum.GetName(typeof(GamePadSide), this.Side))
            .Replace("{1}", this.DeltaX?.ToString(CultureInfo.InvariantCulture) ?? $"<TriggerInputX * {this.InputScaleX}>")
            .Replace("{2}", this.DeltaY?.ToString(CultureInfo.InvariantCulture) ?? $"<TriggerInputY * {this.InputScaleY}>")
            .Replace("{3}", this.GamePadIndex.ToString());

    /// <inheritdoc/>
    public bool Equals(GamePadStickAction other)
        => other is not null
        && this.Side == other.Side
        && this.DeltaX == other.DeltaX
        && this.DeltaY == other.DeltaY
        && this.InputScaleX == other.InputScaleX
        && this.InputScaleY == other.InputScaleY
        && this.GamePadIndex == other.GamePadIndex
        && this.ResetAfterIdleTimeInMs == other.ResetAfterIdleTimeInMs;

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = -2085055944;
        hashCode = (hashCode * -1521134295) + this.Side.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.DeltaX.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.DeltaY.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.InputScaleX.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.InputScaleY.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.GamePadIndex.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.ResetAfterIdleTimeInMs.GetHashCode();
        return hashCode;
    }
}
