namespace Key2Joy.Config;

/// <summary>
/// The type of grouping when listing mapped options in the UI
/// </summary>
public enum ViewMappingGroupType
{
    /// <summary>
    /// No grouping
    /// </summary>
    None = 0,

    /// <summary>
    /// Group by action GroupName's
    /// </summary>
    Action = 1,

    /// <summary>
    /// Group by trigger GroupName's
    /// </summary>
    Trigger = 2,
}
