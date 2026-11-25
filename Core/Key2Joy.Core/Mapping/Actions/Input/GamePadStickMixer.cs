using System;
using System.Collections.Generic;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping.Triggers.GamePad;

namespace Key2Joy.Mapping.Actions.Input;

/// <summary>
/// Combines everything that moves the axes of simulated gamepad sticks into a
/// single value per axis. Values are only clamped to the stick range when they
/// are written to the gamepad, so moves that are later undone (like a key press
/// followed by its release) cancel out exactly, even if the total went beyond
/// the stick range in between.
///
/// Each axis is made of:
/// - A shared total of relative moves (<see cref="AddRelative"/>), used by the exact
///   mode of <see cref="GamePadStickAction"/>. A press and its reverse release are
///   separate actions, so they must add to the same total to cancel out.
/// - A value per source (<see cref="AddToSource"/> and <see cref="SetSource"/>), each
///   clamped to the stick range on its own. These can be removed per source, without
///   affecting what other sources contribute to the same axis.
/// </summary>
public static class GamePadStickMixer
{
    private const short MIN_STICK_VALUE = XInputGamePad.ThumbstickValueMin;
    private const short MAX_STICK_VALUE = XInputGamePad.ThumbstickValueMax;

    private sealed class AxisState
    {
        public long RelativeTotal;
        public readonly Dictionary<object, int> SourceValues = new();
    }

    private readonly record struct AxisKey(int GamePadIndex, GamePadSide Side, GamePadStickAxis Axis);

    private static readonly object syncRoot = new();
    private static readonly Dictionary<AxisKey, AxisState> axes = new();

    /// <summary>
    /// Adds a relative move to the shared total of the axis.
    /// </summary>
    public static void AddRelative(int gamePadIndex, GamePadSide side, GamePadStickAxis axis, int delta)
    {
        lock (syncRoot)
        {
            GetAxis(gamePadIndex, side, axis).RelativeTotal += delta;
        }
    }

    /// <summary>
    /// Resets the shared total of relative moves on the axis to zero.
    /// </summary>
    public static void ResetRelative(int gamePadIndex, GamePadSide side, GamePadStickAxis axis)
    {
        lock (syncRoot)
        {
            if (axes.TryGetValue(new AxisKey(gamePadIndex, side, axis), out var state))
            {
                state.RelativeTotal = 0;
            }
        }
    }

    /// <summary>
    /// Adds to the value of a source on the axis. The value of the source is
    /// clamped to the stick range.
    /// </summary>
    public static void AddToSource(int gamePadIndex, GamePadSide side, GamePadStickAxis axis, object source, int delta)
    {
        lock (syncRoot)
        {
            var state = GetAxis(gamePadIndex, side, axis);
            state.SourceValues.TryGetValue(source, out var current);
            state.SourceValues[source] = Clamp((long)current + delta);
        }
    }

    /// <summary>
    /// Sets the value of a source on the axis. The value is clamped to the stick range.
    /// </summary>
    public static void SetSource(int gamePadIndex, GamePadSide side, GamePadStickAxis axis, object source, int value)
    {
        lock (syncRoot)
        {
            GetAxis(gamePadIndex, side, axis).SourceValues[source] = Clamp(value);
        }
    }

    /// <summary>
    /// Removes the value of a source from the axis.
    /// </summary>
    public static void RemoveSource(int gamePadIndex, GamePadSide side, GamePadStickAxis axis, object source)
    {
        lock (syncRoot)
        {
            if (axes.TryGetValue(new AxisKey(gamePadIndex, side, axis), out var state))
            {
                state.SourceValues.Remove(source);
            }
        }
    }

    /// <summary>
    /// Removes the value of a source from all axes of all gamepads.
    /// </summary>
    public static void RemoveSource(object source)
    {
        lock (syncRoot)
        {
            foreach (var state in axes.Values)
            {
                state.SourceValues.Remove(source);
            }
        }
    }

    /// <summary>
    /// Forgets everything that was contributed to the sticks of a gamepad.
    /// </summary>
    public static void Reset(int gamePadIndex)
    {
        lock (syncRoot)
        {
            foreach (var key in new List<AxisKey>(axes.Keys))
            {
                if (key.GamePadIndex == gamePadIndex)
                {
                    axes.Remove(key);
                }
            }
        }
    }

    /// <summary>
    /// Forgets everything that was contributed to the sticks of all gamepads.
    /// </summary>
    public static void ResetAll()
    {
        lock (syncRoot)
        {
            axes.Clear();
        }
    }

    /// <summary>
    /// Gets the combined value of the axis, clamped to the stick range.
    /// </summary>
    public static short GetValue(int gamePadIndex, GamePadSide side, GamePadStickAxis axis)
    {
        lock (syncRoot)
        {
            return GetValueUnlocked(gamePadIndex, side, axis);
        }
    }

    /// <summary>
    /// Writes the combined values of both axes of the stick to the gamepad and sends
    /// the update.
    /// </summary>
    public static void Apply(ISimulatedGamePad gamePad, GamePadSide side)
    {
        lock (syncRoot)
        {
            var state = gamePad.GetState();
            var x = GetValueUnlocked(gamePad.Index, side, GamePadStickAxis.X);
            var y = GetValueUnlocked(gamePad.Index, side, GamePadStickAxis.Y);

            if (side == GamePadSide.Left)
            {
                state.LeftStickX = x;
                state.LeftStickY = y;
            }
            else
            {
                state.RightStickX = x;
                state.RightStickY = y;
            }

            gamePad.Update();
        }
    }

    private static short GetValueUnlocked(int gamePadIndex, GamePadSide side, GamePadStickAxis axis)
    {
        if (!axes.TryGetValue(new AxisKey(gamePadIndex, side, axis), out var state))
        {
            return 0;
        }

        var total = state.RelativeTotal;

        foreach (var value in state.SourceValues.Values)
        {
            total += value;
        }

        return Clamp(total);
    }

    private static AxisState GetAxis(int gamePadIndex, GamePadSide side, GamePadStickAxis axis)
    {
        var key = new AxisKey(gamePadIndex, side, axis);

        if (!axes.TryGetValue(key, out var state))
        {
            axes.Add(key, state = new AxisState());
        }

        return state;
    }

    private static short Clamp(long value)
        => (short)Math.Min(Math.Max(value, MIN_STICK_VALUE), MAX_STICK_VALUE);
}
