using System;
using System.Threading.Tasks;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Contracts.Mapping.Triggers;

namespace Key2Joy.Mapping.Actions.Logic;

[Action(
    Description = "Cancels an interval previously established by calling SetInterval()",
    Visibility = MappingMenuVisibility.Never,
    GroupName = "Logic",
    GroupImage = "application_xp_terminal"
)]
public class ClearIntervalAction : CoreAction
{
    public ClearIntervalAction(string name)
        : base(name)
    {
    }

    /// <markdown-doc>
    /// <parent-name>Logic</parent-name>
    /// <path>Api/Logic</path>
    /// </markdown-doc>
    /// <summary>
    /// Cancels an interval previously established by calling SetInterval()
    /// </summary>
    /// <markdown-example>
    /// Shows how to count up to 3 every second and then stop by using ClearInterval();
    /// <code language="lua">
    /// <![CDATA[
    /// local count = 0
    /// local intervalId
    ///
    /// intervalId = SetInterval(function ()
    ///    Print(count)
    ///    count = count + 1
    ///
    ///    if (count == 3) then
    ///       ClearInterval(intervalId)
    ///    end
    /// end, 1000)
    ///
    /// Print(intervalId);
    /// ]]>
    /// </code>
    /// </markdown-example>
    /// <name>ClearInterval</name>
    /// <param name="intervalId">Id returned by SetInterval to cancel</param>
    [ExposesScriptingMethod("ClearInterval")]
    public void ExecuteForScript(IdPool.IntervalId intervalId) => intervalId.Cancel();

    public override Task Execute(AbstractInputBag inputBag = null) =>
        // Irrelevant because only scripts should use this function
        null;

    public override string GetNameDisplay() =>
        // Irrelevant because only scripts should use this function
        null;
}
