using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping.Actions.Input;
using Key2Joy.Mapping.Triggers.GamePad;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Key2Joy.Tests.Core.Mapping.Actions.Input;

[TestClass]
public class GamePadStickMixerTests
{
    private const int PAD = 0;
    private const short MAX = XInputGamePad.ThumbstickValueMax;
    private const short MIN = XInputGamePad.ThumbstickValueMin;
    private const short HALF = MAX / 2;

    [TestInitialize]
    public void Initialize() => GamePadStickMixer.ResetAll();

    [TestCleanup]
    public void Cleanup() => GamePadStickMixer.ResetAll();

    private static short X(GamePadSide side = GamePadSide.Left)
        => GamePadStickMixer.GetValue(PAD, side, GamePadStickAxis.X);

    private static short Y(GamePadSide side = GamePadSide.Left)
        => GamePadStickMixer.GetValue(PAD, side, GamePadStickAxis.Y);

    private static void AddRelativeX(int delta)
        => GamePadStickMixer.AddRelative(PAD, GamePadSide.Left, GamePadStickAxis.X, delta);

    [TestMethod]
    public void Untouched_Axis_Is_Centered()
        => Assert.AreEqual(0, X());

    [TestMethod]
    public void Relative_Press_And_Release_Cancel_Out()
    {
        AddRelativeX(HALF);
        Assert.AreEqual(HALF, X());

        AddRelativeX(-HALF);
        Assert.AreEqual(0, X());
    }

    [TestMethod]
    public void Relative_Opposite_Holds_Cancel_And_Restore()
    {
        // Hold D (+0.5), then hold A (-0.5), then release D
        AddRelativeX(HALF);
        AddRelativeX(-HALF);
        Assert.AreEqual(0, X());

        AddRelativeX(-HALF);
        Assert.AreEqual(-HALF, X());
    }

    [TestMethod]
    public void Relative_Total_Is_Only_Clamped_On_Output()
    {
        // Two keys pushing right fully, then one released: the other should still push fully
        AddRelativeX(MAX);
        AddRelativeX(MAX);
        Assert.AreEqual(MAX, X());

        AddRelativeX(-MAX);
        Assert.AreEqual(MAX, X());

        AddRelativeX(-MAX);
        Assert.AreEqual(0, X());
    }

    [TestMethod]
    public void ResetRelative_Only_Affects_That_Axis()
    {
        AddRelativeX(HALF);
        GamePadStickMixer.AddRelative(PAD, GamePadSide.Left, GamePadStickAxis.Y, HALF);

        GamePadStickMixer.ResetRelative(PAD, GamePadSide.Left, GamePadStickAxis.X);

        Assert.AreEqual(0, X());
        Assert.AreEqual(HALF, Y());
    }

    [TestMethod]
    public void Source_Value_Is_Clamped_Per_Source()
    {
        var mouse = new object();

        // Lots of movement in one direction must not wind up beyond the stick range,
        // otherwise moving back would take just as long before the stick responds.
        for (var i = 0; i < 10; i++)
        {
            GamePadStickMixer.AddToSource(PAD, GamePadSide.Left, GamePadStickAxis.X, mouse, MAX);
        }

        GamePadStickMixer.AddToSource(PAD, GamePadSide.Left, GamePadStickAxis.X, mouse, -HALF);

        Assert.AreEqual(MAX - HALF, X());
    }

    [TestMethod]
    public void Removing_Source_Leaves_Other_Contributions()
    {
        var mouse = new object();

        AddRelativeX(-HALF); // Held key
        GamePadStickMixer.AddToSource(PAD, GamePadSide.Left, GamePadStickAxis.X, mouse, HALF);
        Assert.AreEqual(0, X());

        GamePadStickMixer.RemoveSource(mouse);

        Assert.AreEqual(-HALF, X());
    }

    [TestMethod]
    public void SetSource_Replaces_Value()
    {
        var trigger = new object();

        GamePadStickMixer.SetSource(PAD, GamePadSide.Left, GamePadStickAxis.X, trigger, MAX);
        GamePadStickMixer.SetSource(PAD, GamePadSide.Left, GamePadStickAxis.X, trigger, HALF);

        Assert.AreEqual(HALF, X());
    }

    [TestMethod]
    public void Combined_Output_Is_Clamped()
    {
        var mouse = new object();

        AddRelativeX(MIN);
        GamePadStickMixer.AddToSource(PAD, GamePadSide.Left, GamePadStickAxis.X, mouse, MIN);

        Assert.AreEqual(MIN, X());
    }

    [TestMethod]
    public void Sides_And_GamePads_Are_Independent()
    {
        AddRelativeX(HALF);

        Assert.AreEqual(0, X(GamePadSide.Right));
        Assert.AreEqual(0, GamePadStickMixer.GetValue(PAD + 1, GamePadSide.Left, GamePadStickAxis.X));
    }

    [TestMethod]
    public void Reset_Forgets_Everything_On_That_GamePad_Only()
    {
        var mouse = new object();

        AddRelativeX(HALF);
        GamePadStickMixer.AddToSource(PAD, GamePadSide.Right, GamePadStickAxis.Y, mouse, HALF);
        GamePadStickMixer.AddRelative(PAD + 1, GamePadSide.Left, GamePadStickAxis.X, HALF);

        GamePadStickMixer.Reset(PAD);

        Assert.AreEqual(0, X());
        Assert.AreEqual(0, Y(GamePadSide.Right));
        Assert.AreEqual(HALF, GamePadStickMixer.GetValue(PAD + 1, GamePadSide.Left, GamePadStickAxis.X));
    }
}
