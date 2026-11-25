using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping.Actions.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Key2Joy.Tests.Core.Mapping.Actions.Input;

[TestClass]
public class StickSpeedTrackerTests
{
    // Use milliseconds as timestamp ticks to keep the numbers readable
    private const long TICKS_PER_SECOND = 1000;
    private const long WINDOW = 20;
    private const short MAX = XInputGamePad.ThumbstickValueMax;

    [TestMethod]
    public void No_Samples_Means_No_Speed()
    {
        var tracker = new StickSpeedTracker();

        Assert.AreEqual((0d, 0d), tracker.GetSpeed(100, WINDOW, TICKS_PER_SECOND));
        Assert.IsFalse(tracker.HasSamples);
    }

    [TestMethod]
    public void Speed_Is_Movement_In_Window_Per_Second()
    {
        var tracker = new StickSpeedTracker();

        // 20 units over a 20 ms window = 1000 units per second
        for (var t = 0; t < 20; t++)
        {
            tracker.Add(t, 1, -2);
        }

        var (x, y) = tracker.GetSpeed(19, WINDOW, TICKS_PER_SECOND);

        Assert.AreEqual(1000, x, 0.001);
        Assert.AreEqual(-2000, y, 0.001);
    }

    [TestMethod]
    public void Old_Movement_Leaves_The_Window()
    {
        var tracker = new StickSpeedTracker();

        tracker.Add(0, 10, 0);
        tracker.Add(15, 10, 0);

        Assert.AreEqual(1000, tracker.GetSpeed(15, WINDOW, TICKS_PER_SECOND).X, 0.001);

        // The first sample is now 20 ms old
        Assert.AreEqual(500, tracker.GetSpeed(20, WINDOW, TICKS_PER_SECOND).X, 0.001);
        Assert.IsTrue(tracker.HasSamples);
    }

    [TestMethod]
    public void Speed_Drops_To_Zero_After_Movement_Stops()
    {
        var tracker = new StickSpeedTracker();

        tracker.Add(0, 0.1, 0.3);
        tracker.Add(5, 0.2, 0.3);

        var (x, y) = tracker.GetSpeed(25, WINDOW, TICKS_PER_SECOND);

        // Exactly zero, without floating point leftovers that would give minimum output
        Assert.AreEqual(0d, x);
        Assert.AreEqual(0d, y);
        Assert.IsFalse(tracker.HasSamples);
    }

    [TestMethod]
    public void Clear_Forgets_Movement()
    {
        var tracker = new StickSpeedTracker();

        tracker.Add(0, 10, 10);
        tracker.Clear();

        Assert.AreEqual((0d, 0d), tracker.GetSpeed(1, WINDOW, TICKS_PER_SECOND));
    }

    [TestMethod]
    public void Map_Zero_Speed_Is_Centered_Even_With_Minimum_Output()
        => Assert.AreEqual(0, StickSpeedTracker.MapSpeedToStick(0, 2000, 1, 0.3));

    [TestMethod]
    public void Map_Is_Linear_With_Curve_One()
    {
        Assert.AreEqual((short)(MAX / 2.0 + 0.5), StickSpeedTracker.MapSpeedToStick(1000, 2000, 1, 0));
        Assert.AreEqual((short)-(MAX / 2.0 + 0.5), StickSpeedTracker.MapSpeedToStick(-1000, 2000, 1, 0));
    }

    [TestMethod]
    public void Map_Clamps_Above_Full_Deflection()
    {
        Assert.AreEqual(MAX, StickSpeedTracker.MapSpeedToStick(10000, 2000, 1, 0));
        Assert.AreEqual(-MAX, StickSpeedTracker.MapSpeedToStick(-10000, 2000, 1, 0));
    }

    [TestMethod]
    public void Map_Curve_Gives_Finer_Control_At_Low_Speed()
    {
        var linear = StickSpeedTracker.MapSpeedToStick(500, 2000, 1, 0);
        var curved = StickSpeedTracker.MapSpeedToStick(500, 2000, 2, 0);

        Assert.IsTrue(curved < linear);
        Assert.AreEqual((short)System.Math.Round(0.0625 * MAX), curved);
    }

    [TestMethod]
    public void Map_Minimum_Output_Lifts_Small_Speeds_And_Keeps_Full()
    {
        Assert.AreEqual((short)System.Math.Round(0.3 * MAX), StickSpeedTracker.MapSpeedToStick(0.01, 2000, 1, 0.3));
        Assert.AreEqual(MAX, StickSpeedTracker.MapSpeedToStick(2000, 2000, 1, 0.3));
    }
}
