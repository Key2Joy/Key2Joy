using System;
using Key2Joy.Contracts.Mapping.Triggers;

namespace Key2Joy.Mapping.Triggers;

/// <summary>
/// Defines the contract for a control that manages options for a trigger in a user interface.
/// </summary>
public interface ITriggerOptionsControl
{
    /// <summary>
    /// Called to setup the options panel with a trigger
    /// </summary>
    /// <param name="trigger"></param>
    public void Select(AbstractTrigger trigger);

    /// <summary>
    /// Called when the options panel should modify a resulting trigger
    /// </summary>
    /// <param name="trigger"></param>
    public void Setup(AbstractTrigger trigger);

    /// <summary>
    /// Called when the mapping is saving and can still be stopped
    /// </summary>
    public bool CanMappingSave(AbstractTrigger trigger);

    /// <summary>
    /// Called when the options on a trigger change
    /// </summary>
    public event EventHandler? OptionsChanged;
}
