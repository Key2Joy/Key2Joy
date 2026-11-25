using System;
using System.Collections.Generic;
using Key2Joy.LowLevelInput.XInput;

namespace Key2Joy.Mapping.Actions.Input;

/// <summary>
/// Tracks relative movement (e.g. of the mouse) over a sliding time window, to find
/// its speed. When movement stops, the samples leave the window and the speed drops
/// to zero over the length of the window. Not thread-safe.
/// </summary>
public sealed class StickSpeedTracker
{
    /// <summary>
    /// Speeds (in units per second) below this are treated as no movement, so rounding
    /// errors don't produce (minimum) output.
    /// </summary>
    private const double SPEED_EPSILON = 0.001;

    private readonly Queue<(long Timestamp, double X, double Y)> samples = new();
    private double sumX;
    private double sumY;

    /// <summary>
    /// Whether there is movement in the window (as of the last call to <see cref="GetSpeed"/>).
    /// </summary>
    public bool HasSamples => this.samples.Count > 0;

    /// <summary>
    /// Adds movement that happened at the given timestamp.
    /// </summary>
    public void Add(long timestamp, double x, double y)
    {
        this.samples.Enqueue((timestamp, x, y));
        this.sumX += x;
        this.sumY += y;
    }

    /// <summary>
    /// Forgets all movement.
    /// </summary>
    public void Clear()
    {
        this.samples.Clear();
        this.sumX = 0;
        this.sumY = 0;
    }

    /// <summary>
    /// Gets the average speed (in units per second) over the window that ends at
    /// <paramref name="now"/>, dropping movement older than the window.
    /// </summary>
    /// <param name="now">Current timestamp</param>
    /// <param name="windowTicks">Length of the window, in timestamp ticks</param>
    /// <param name="ticksPerSecond">How many timestamp ticks make a second</param>
    public (double X, double Y) GetSpeed(long now, long windowTicks, long ticksPerSecond)
    {
        windowTicks = Math.Max(1, windowTicks);

        while (this.samples.Count > 0 && now - this.samples.Peek().Timestamp >= windowTicks)
        {
            var sample = this.samples.Dequeue();
            this.sumX -= sample.X;
            this.sumY -= sample.Y;
        }

        if (this.samples.Count == 0)
        {
            // Prevent floating point leftovers from building up
            this.sumX = 0;
            this.sumY = 0;
            return (0, 0);
        }

        var seconds = (double)windowTicks / ticksPerSecond;
        return (this.sumX / seconds, this.sumY / seconds);
    }

    /// <summary>
    /// Maps a speed to a stick value.
    /// </summary>
    /// <param name="speed">Speed in units per second, negative for the other direction</param>
    /// <param name="fullDeflectionSpeed">Speed at which the stick is fully deflected</param>
    /// <param name="curve">1 is linear, higher gives finer control at low speeds</param>
    /// <param name="minimumOutput">Fraction (0 to 1) of deflection given for any movement,
    /// to get past deadzones in the game or emulator</param>
    public static short MapSpeedToStick(double speed, double fullDeflectionSpeed, double curve, double minimumOutput)
    {
        if (double.IsNaN(speed) || Math.Abs(speed) < SPEED_EPSILON)
        {
            return 0;
        }

        fullDeflectionSpeed = Math.Max(1, fullDeflectionSpeed);
        curve = Math.Max(0.1, curve);
        minimumOutput = Math.Min(Math.Max(minimumOutput, 0), 1);

        var fraction = Math.Min(Math.Abs(speed) / fullDeflectionSpeed, 1);
        fraction = Math.Pow(fraction, curve);

        var output = minimumOutput + ((1 - minimumOutput) * fraction);

        return (short)Math.Round(Math.Sign(speed) * output * XInputGamePad.ThumbstickValueMax);
    }
}
