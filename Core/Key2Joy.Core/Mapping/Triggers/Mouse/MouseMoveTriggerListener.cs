using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Key2Joy.Config;
using Key2Joy.Contracts;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.LowLevelInput;
using Key2Joy.Util;

namespace Key2Joy.Mapping.Triggers.Mouse;

public class MouseMoveTriggerListener : CoreTriggerListener, IOverrideDefaultBehavior
{
    public static MouseMoveTriggerListener instance;

    public static MouseMoveTriggerListener Instance
    {
        get
        {
            instance ??= new MouseMoveTriggerListener();

            return instance;
        }
    }

    private static readonly TimeSpan IS_MOVING_TOLERANCE = TimeSpan.FromMilliseconds(10);

    private readonly Dictionary<int, List<AbstractMappedOption>> lookupAxis;

    private GlobalInputHook globalMouseButtonHook;
    private List<int> lastDirectionHashes = new();
    private DateTime lastMoveTime;

    /// <summary>
    /// Only set when raw mouse movement is used (see <see cref="ConfigState.UseRawMouseMovement"/>)
    /// </summary>
    private RawMouseInput rawMouseInput;
    private SynchronizationContext startContext;
    private readonly object rawMovementLock = new();
    private int pendingRawX;
    private int pendingRawY;
    private bool isRawFlushPending;

    private MouseMoveTriggerListener()
        => this.lookupAxis = new Dictionary<int, List<AbstractMappedOption>>();

    /// <inheritdoc/>
    public bool ShouldListenerOverrideDefault(bool executedAny)
    {
        var configManager = ServiceContainer.Get<IConfigManager>();
        var config = configManager.GetConfigState();
        var listenerOverrideDefaultAll = config.ListenerOverrideDefaultMouseMoveAll;

        if (listenerOverrideDefaultAll)
        {
            return true;
        }

        var listenerOverrideDefault = config.ListenerOverrideDefaultMouseMove;
        return listenerOverrideDefault && executedAny;
    }

    /// <inheritdoc/>
    protected override void Start()
    {
        // This captures global mouse input and blocks default behaviour by setting e.Handled
        this.globalMouseButtonHook = new GlobalInputHook();
        this.globalMouseButtonHook.MouseInputEvent += this.OnMouseInputEvent;

        var config = ServiceContainer.Get<IConfigManager>().GetConfigState();

        if (config.UseRawMouseMovement)
        {
            try
            {
                this.startContext = SynchronizationContext.Current;
                this.rawMouseInput = new RawMouseInput();
                this.rawMouseInput.MouseMoved += this.OnRawMouseMoved;
            }
            catch (Exception ex)
            {
                // Fall back to measuring movement with the hook
                Output.WriteLine(Output.OutputModes.Default, $"Raw mouse input unavailable, using the mouse hook instead: {ex.Message}");
                this.rawMouseInput = null;
            }
        }

        base.Start();
    }

    /// <inheritdoc/>
    protected override void Stop()
    {
        instance = null;
        this.globalMouseButtonHook.MouseInputEvent -= this.OnMouseInputEvent;
        this.globalMouseButtonHook.Dispose();
        this.globalMouseButtonHook = null;

        if (this.rawMouseInput != null)
        {
            this.rawMouseInput.MouseMoved -= this.OnRawMouseMoved;
            this.rawMouseInput.Dispose();
            this.rawMouseInput = null;
        }

        base.Stop();
    }

    /// <summary>
    /// No longer needed: movement is now measured from the actual cursor position, so
    /// games that move the cursor no longer cause drift. Kept so existing profiles that
    /// use <see cref="Actions.Logic.AppCommand.ResetMouseMoveTriggerCenter"/> keep working.
    /// </summary>
    public void ResetCenterCursor()
        => System.Diagnostics.Debug.WriteLine($"Mouse move reset center (no longer needed)");

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetPhysicalCursorPos(out Point lpPoint);

    /// <inheritdoc/>
    public override void AddMappedOption(AbstractMappedOption mappedOption)
    {
        var trigger = mappedOption.Trigger as MouseMoveTrigger;

        if (!this.lookupAxis.TryGetValue(trigger.GetInputHash(), out var mappedOptions))
        {
            this.lookupAxis.Add(trigger.GetInputHash(), mappedOptions = new List<AbstractMappedOption>());
        }

        mappedOptions.Add(mappedOption);
    }

    /// <inheritdoc/>
    public override bool GetIsTriggered(AbstractTrigger trigger)
    {
        if (trigger is not MouseMoveTrigger mouseMoveTrigger)
        {
            return false;
        }

        return DateTime.Now - this.lastMoveTime < IS_MOVING_TOLERANCE
            && this.lastDirectionHashes.Contains(mouseMoveTrigger.GetInputHash());
    }

    private void OnMouseInputEvent(object sender, GlobalMouseHookEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }

        /// Mouse buttons are handled through <see cref="MouseButtonTriggerListener"/>
        if (e.MouseState != MouseState.Move)
        {
            return;
        }

        // The hook is called before the cursor moves, so the movement is the difference
        // between where the cursor will go and where it is now. Measuring from the actual
        // cursor position (instead of the last position we saw) means it doesn't matter
        // whether we block the movement, or whether something else (like a game) moved
        // the cursor in the meantime.
        if (!GetPhysicalCursorPos(out var cursorPosition))
        {
            return;
        }

        var deltaX = e.RawData.Position.X - cursorPosition.X;
        var deltaY = e.RawData.Position.Y - cursorPosition.Y;

        if (this.rawMouseInput != null)
        {
            // Mappings are executed with the raw movement (see OnRawMouseMoved). The hook
            // only decides whether the cursor movement is blocked, based on whether there
            // are mappings for the direction it moves in.
            var mappedOptions = this.FindMappedOptions(deltaX, deltaY, out _);

            if (this.ShouldListenerOverrideDefault(mappedOptions.Count > 0))
            {
                e.Handled = true;
            }

            return;
        }

        if (this.ExecuteMovement(deltaX, deltaY))
        {
            e.Handled = true;
        }
    }

    /// <summary>
    /// Called on the Raw Input thread. Movement is collected and handed to the thread the
    /// listener was started on (like the hook, which runs there), so mappings keep running
    /// on the same thread. Movement that arrives before that thread handled the previous
    /// movement is combined.
    /// </summary>
    private void OnRawMouseMoved(int deltaX, int deltaY)
    {
        var context = this.startContext;

        if (context == null)
        {
            this.ExecuteRawMovement(deltaX, deltaY);
            return;
        }

        lock (this.rawMovementLock)
        {
            this.pendingRawX += deltaX;
            this.pendingRawY += deltaY;

            if (this.isRawFlushPending)
            {
                return;
            }

            this.isRawFlushPending = true;
        }

        context.Post(_ => this.FlushRawMovement(), null);
    }

    private void FlushRawMovement()
    {
        int deltaX;
        int deltaY;

        lock (this.rawMovementLock)
        {
            deltaX = this.pendingRawX;
            deltaY = this.pendingRawY;
            this.pendingRawX = 0;
            this.pendingRawY = 0;
            this.isRawFlushPending = false;
        }

        this.ExecuteRawMovement(deltaX, deltaY);
    }

    private void ExecuteRawMovement(int deltaX, int deltaY)
    {
        if (!this.IsActive || (deltaX == 0 && deltaY == 0))
        {
            return;
        }

        this.ExecuteMovement(deltaX, deltaY);
    }

    /// <summary>
    /// Finds the mappings for the directions of the movement.
    /// </summary>
    private List<AbstractMappedOption> FindMappedOptions(int deltaX, int deltaY, out List<int> directionHashes)
    {
        List<AbstractMappedOption> mappedOptions = new();
        directionHashes = new();
        Dictionary<int, bool> directionChecks = new()
        {
            { MouseMoveTrigger.GetInputHashFor(AxisDirection.Any), deltaX != 0 || deltaY != 0 },
            { MouseMoveTrigger.GetInputHashFor(AxisDirection.Right), deltaX > 0 },
            { MouseMoveTrigger.GetInputHashFor(AxisDirection.Left), deltaX < 0 },
            { MouseMoveTrigger.GetInputHashFor(AxisDirection.Forward), deltaY > 0 },
            { MouseMoveTrigger.GetInputHashFor(AxisDirection.Backward), deltaY < 0 },
        };

        foreach (var directionCheck in directionChecks)
        {
            if (directionCheck.Value
                && this.lookupAxis.TryGetValue(directionCheck.Key, out var matchedOptions))
            {
                mappedOptions.AddRange(matchedOptions);
                directionHashes.Add(directionCheck.Key);
            }
        }

        return mappedOptions;
    }

    /// <summary>
    /// Executes the mappings for the movement.
    /// </summary>
    /// <returns>Whether the default behavior (moving the cursor) should be blocked</returns>
    private bool ExecuteMovement(int deltaX, int deltaY)
    {
        var mappedOptions = this.FindMappedOptions(deltaX, deltaY, out var directionHashes);

        AxisDeltaInputBag inputBag = new()
        {
            DeltaX = deltaX,
            DeltaY = deltaY,
        };

        var shouldOverride = this.DoExecuteTrigger(
            mappedOptions,
            inputBag,
            trigger => directionHashes.Contains((trigger as IReturnInputHash).GetInputHash())
        );

        this.lastDirectionHashes = directionHashes;
        this.lastMoveTime = DateTime.Now;

        return shouldOverride;
    }
}
