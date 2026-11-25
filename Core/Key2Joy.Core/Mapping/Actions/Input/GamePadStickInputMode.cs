namespace Key2Joy.Mapping.Actions.Input;

/// <summary>
/// How a <see cref="GamePadStickAction"/> turns relative input (like mouse movement)
/// into stick movement.
/// </summary>
public enum GamePadStickInputMode
{
    /// <summary>
    /// Movement is added to the stick, until it's reset after being idle. The further you
    /// move, the further the stick goes. Suits games where the stick position is the aim.
    /// </summary>
    Accumulate = 0,

    /// <summary>
    /// The stick follows the speed of the movement, returning to the center when the
    /// movement stops. Suits games where the stick sets how fast you turn.
    /// </summary>
    Speed = 1,
}
