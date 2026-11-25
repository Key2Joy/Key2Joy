namespace Key2Joy.Contracts.Mapping.Actions;

/// <summary>
/// Optionally implemented by an <see cref="IActionOptionsControl"/> that needs to know
/// whether the mapping being edited also creates a reverse mapping (e.g. a key release
/// that undoes what the key press did).
/// </summary>
public interface IReverseMappingAwareOptionsControl
{
    /// <summary>
    /// Called when it changes whether the mapping also creates a reverse mapping. Also
    /// called right after the options control is created. Changes this causes to the
    /// action should be applied in <see cref="IActionOptionsControl.Setup"/>, which is
    /// called after this.
    /// </summary>
    /// <param name="createsReverseMapping"></param>
    void SetCreatesReverseMapping(bool createsReverseMapping);
}
