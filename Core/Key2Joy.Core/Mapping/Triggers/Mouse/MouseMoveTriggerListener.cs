using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Key2Joy.Config;
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
    private List<int> lastDirectionHashes;
    private DateTime lastMoveTime;

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

        base.Start();
    }

    /// <inheritdoc/>
    protected override void Stop()
    {
        instance = null;
        this.globalMouseButtonHook.MouseInputEvent -= this.OnMouseInputEvent;
        this.globalMouseButtonHook.Dispose();
        this.globalMouseButtonHook = null;

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

        List<AbstractMappedOption> mappedOptions = new();
        List<int> directionHashes = new();
        Dictionary<int, Func<bool>> directionChecks = new()
        {
            {
                MouseMoveTrigger.GetInputHashFor(AxisDirection.Any),
                () => deltaX != 0 || deltaY != 0
            },
            {
                MouseMoveTrigger.GetInputHashFor(AxisDirection.Right),
                () => deltaX > 0
            },
            {
                MouseMoveTrigger.GetInputHashFor(AxisDirection.Left),
                () => deltaX < 0
            },
            {
                MouseMoveTrigger.GetInputHashFor(AxisDirection.Forward),
                () => deltaY > 0
            },
            {
                MouseMoveTrigger.GetInputHashFor(AxisDirection.Backward),
                () => deltaY < 0
            },
        };

        foreach (var directionCheck in directionChecks)
        {
            if (directionCheck.Value.Invoke())
            {
                if (this.lookupAxis.TryGetValue(directionCheck.Key, out var matchedOptions))
                {
                    mappedOptions.AddRange(matchedOptions);
                    directionHashes.Add(directionCheck.Key);
                }
            }
        }

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

        if (shouldOverride)
        {
            e.Handled = true;
        }
    }
}
