using System;
using System.Collections.Generic;
using System.Diagnostics;
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
public class GamePadStickAction : CoreAction, IProvideReverseAspect, IEquatable<GamePadStickAction>, IStickSpeedTickTarget
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
    /// How relative input (like mouse movement) moves the stick, when DeltaX and DeltaY are null.
    /// </summary>
    public GamePadStickInputMode InputMode { get; set; } = GamePadStickInputMode.Accumulate;

    /// <summary>
    /// In <see cref="GamePadStickInputMode.Speed"/> mode: the speed of the input (in units,
    /// like mouse counts, per second, after applying the input scale) that fully deflects
    /// the stick.
    /// </summary>
    public float SpeedFullDeflection { get; set; } = 2000;

    /// <summary>
    /// In <see cref="GamePadStickInputMode.Speed"/> mode: over how many milliseconds the
    /// speed is averaged. Higher is smoother, but slower to respond. This is also how long
    /// the stick takes to return to the center after the input stops.
    /// </summary>
    public int SpeedSmoothingMs { get; set; } = 20;

    /// <summary>
    /// In <see cref="GamePadStickInputMode.Speed"/> mode: 1 maps speed to stick linearly,
    /// higher values give finer control at low speeds.
    /// </summary>
    public float SpeedCurve { get; set; } = 1;

    /// <summary>
    /// In <see cref="GamePadStickInputMode.Speed"/> mode: the fraction (0 to 1) of deflection
    /// any movement gives, to get past deadzones in the game or emulator.
    /// </summary>
    public float SpeedMinimumOutput { get; set; } = 0;

    /// <summary>
    /// After how many milliseconds without this action executing should what it
    /// moved be undone? 0 or less means never. Not used in
    /// <see cref="GamePadStickInputMode.Speed"/> mode, which returns the stick by itself.
    /// </summary>
    public int ResetAfterIdleTimeInMs { get; set; } = 500;

    /// <summary>
    /// Which gamepad to simulate
    /// </summary>
    public int GamePadIndex { get; set; }

    private System.Timers.Timer noInputTimer;

    /// <summary>
    /// Speed mode state, guarded by locking <see cref="speedTracker"/>
    /// </summary>
    private StickSpeedTracker speedTracker;
    private short lastSpeedX;
    private short lastSpeedY;

    /// <summary>
    /// Axes this action added relative (exact mode) moves to since the last reset.
    /// </summary>
    private HashSet<(int GamePadIndex, GamePadSide Side, GamePadStickAxis Axis)> movedRelativeAxes;

    /// <summary>
    /// Sticks this action contributed to since the last reset, so they can be updated
    /// when the contributions are undone.
    /// </summary>
    private HashSet<(int GamePadIndex, GamePadSide Side)> touchedSticks;

    /// <summary>
    /// Accumulated relative input values this action added to since the last reset, with
    /// the source they're stored under (shared per trigger listener).
    /// </summary>
    private HashSet<(int GamePadIndex, GamePadSide Side, GamePadStickAxis Axis, object Source)> accumulatedSources;

    public GamePadStickAction(string name) : base(name)
        => this.InitializeResetState();

    private void InitializeResetState()
    {
        this.noInputTimer = new System.Timers.Timer();
        this.noInputTimer.Elapsed += this.NoInputTimer_Elapsed;
        this.noInputTimer.AutoReset = false;
        this.movedRelativeAxes = new();
        this.accumulatedSources = new();
        this.touchedSticks = new();
        this.speedTracker = new();
        this.lastSpeedX = 0;
        this.lastSpeedY = 0;
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
        this.StopSpeed();
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

        if (this.DeltaX is null
            && this.DeltaY is null
            && this.InputMode == GamePadStickInputMode.Speed
            && inputBag is AxisDeltaInputBag speedInputBag)
        {
            this.AddSpeedSample(speedInputBag);
            return;
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
    /// Records movement for speed mode. The stick itself is updated by the
    /// <see cref="StickSpeedTicker"/>, which keeps updating it while the speed decays.
    /// </summary>
    private void AddSpeedSample(AxisDeltaInputBag inputBag)
    {
        lock (this.speedTracker)
        {
            this.speedTracker.Add(
                Stopwatch.GetTimestamp(),
                inputBag.DeltaX * (double)this.InputScaleX,
                inputBag.DeltaY * (double)this.InputScaleY
            );
        }

        this.TrackTouchedStick();
        StickSpeedTicker.Activate(this);
    }

    /// <inheritdoc/>
    bool IStickSpeedTickTarget.HasPendingSpeedWork
    {
        get
        {
            lock (this.speedTracker)
            {
                return this.speedTracker.HasSamples
                    || this.lastSpeedX != 0
                    || this.lastSpeedY != 0;
            }
        }
    }

    /// <inheritdoc/>
    void IStickSpeedTickTarget.OnSpeedTick(long now)
    {
        short x;
        short y;

        lock (this.speedTracker)
        {
            var windowTicks = Stopwatch.Frequency * Math.Max(1, this.SpeedSmoothingMs) / 1000;
            var (speedX, speedY) = this.speedTracker.GetSpeed(now, windowTicks, Stopwatch.Frequency);

            x = StickSpeedTracker.MapSpeedToStick(speedX, this.SpeedFullDeflection, this.SpeedCurve, this.SpeedMinimumOutput);
            y = StickSpeedTracker.MapSpeedToStick(speedY, this.SpeedFullDeflection, this.SpeedCurve, this.SpeedMinimumOutput);

            if (x == this.lastSpeedX && y == this.lastSpeedY)
            {
                return;
            }

            this.lastSpeedX = x;
            this.lastSpeedY = y;

            // Published while holding the lock, so StopSpeed (which takes the same lock)
            // can't run in between and leave these values behind after cleanup.
            GamePadStickMixer.SetSource(this.GamePadIndex, this.Side, GamePadStickAxis.X, this, x);
            GamePadStickMixer.SetSource(this.GamePadIndex, this.Side, GamePadStickAxis.Y, this, y);
        }

        var gamePad = ServiceContainer.Get<ISimulatedGamePadService>().GetGamePad(this.GamePadIndex);

        if (gamePad.GetIsPluggedIn())
        {
            GamePadStickMixer.Apply(gamePad, this.Side);
        }
    }

    /// <summary>
    /// Stops speed mode updates and forgets the movement.
    /// </summary>
    private void StopSpeed()
    {
        StickSpeedTicker.Deactivate(this);

        lock (this.speedTracker)
        {
            this.speedTracker.Clear();
            this.lastSpeedX = 0;
            this.lastSpeedY = 0;
        }
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
            // Relative input (e.g. mouse movement) accumulates in a clamped value that is
            // separate from other kinds of input on this axis, so undoing it doesn't affect
            // e.g. held keys. Mappings fed by the same trigger listener share that value, so
            // separate directional mappings (like Mouse Move Left and Right) can fully undo
            // each other, like they did when they shared the stick directly.
            var source = (object)this.Listener ?? this;
            var delta = isX
                ? Scale(axisInputBag.DeltaX, this.InputScaleX)
                : Scale(axisInputBag.DeltaY, this.InputScaleY);

            if (delta == 0)
            {
                return false;
            }

            GamePadStickMixer.AddToSource(this.GamePadIndex, this.Side, axis, source, delta);

            lock (this.movedRelativeAxes)
            {
                this.accumulatedSources.Add((this.GamePadIndex, this.Side, axis, source));
                this.touchedSticks.Add((this.GamePadIndex, this.Side));
            }

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
    /// - Its own values (speed, gamepad triggers) are removed, leaving other mappings alone.
    /// - Accumulated relative input is removed on the axes this action moved. That value is
    ///   shared with mappings fed by the same trigger listener (e.g. Mouse Move Left and
    ///   Right), so they are undone together, like the reset always behaved.
    /// - The shared total of exact moves is reset on the axes this action moved. That
    ///   total is shared with the reverse mapping (e.g. key release), so it can't be
    ///   undone per action. This matches how the reset always behaved.
    /// </summary>
    private void UndoContributions()
    {
        List<(int GamePadIndex, GamePadSide Side, GamePadStickAxis Axis)> relativeAxes;
        List<(int GamePadIndex, GamePadSide Side, GamePadStickAxis Axis, object Source)> sources;
        List<(int GamePadIndex, GamePadSide Side)> sticks;

        lock (this.movedRelativeAxes)
        {
            relativeAxes = new(this.movedRelativeAxes);
            sources = new(this.accumulatedSources);
            sticks = new(this.touchedSticks);
            this.movedRelativeAxes.Clear();
            this.accumulatedSources.Clear();
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

        foreach (var (gamePadIndex, side, axis, source) in sources)
        {
            GamePadStickMixer.RemoveSource(gamePadIndex, side, axis, source);
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
    {
        var inputName = this.InputMode == GamePadStickInputMode.Speed ? "TriggerSpeed" : "TriggerInput";

        return this.Name.Replace("{0}", Enum.GetName(typeof(GamePadSide), this.Side))
            .Replace("{1}", this.DeltaX?.ToString(CultureInfo.InvariantCulture) ?? $"<{inputName}X * {this.InputScaleX}>")
            .Replace("{2}", this.DeltaY?.ToString(CultureInfo.InvariantCulture) ?? $"<{inputName}Y * {this.InputScaleY}>")
            .Replace("{3}", this.GamePadIndex.ToString());
    }

    /// <inheritdoc/>
    public bool Equals(GamePadStickAction other)
        => other is not null
        && this.Side == other.Side
        && this.DeltaX == other.DeltaX
        && this.DeltaY == other.DeltaY
        && this.InputScaleX == other.InputScaleX
        && this.InputScaleY == other.InputScaleY
        && this.InputMode == other.InputMode
        && this.SpeedFullDeflection == other.SpeedFullDeflection
        && this.SpeedSmoothingMs == other.SpeedSmoothingMs
        && this.SpeedCurve == other.SpeedCurve
        && this.SpeedMinimumOutput == other.SpeedMinimumOutput
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
        hashCode = (hashCode * -1521134295) + this.InputMode.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.SpeedFullDeflection.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.SpeedSmoothingMs.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.SpeedCurve.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.SpeedMinimumOutput.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.GamePadIndex.GetHashCode();
        hashCode = (hashCode * -1521134295) + this.ResetAfterIdleTimeInMs.GetHashCode();
        return hashCode;
    }
}
