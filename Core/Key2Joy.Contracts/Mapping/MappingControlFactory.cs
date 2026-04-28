using System;

namespace Key2Joy.Contracts.Mapping;

/// <summary>
/// Creates instances of the Control, simply using Activator.CreateInstance
/// </summary>
public abstract class MappingControlFactory
{
    public string ForTypeFullName { get; private set; }
    public string TextGlyph { get; private set; }

    public MappingControlFactory(string forTypeFullName, string textGlyph)
    {
        this.ForTypeFullName = forTypeFullName;
        this.TextGlyph = textGlyph;
    }

    public virtual T CreateInstance<T>() => (T)Activator.CreateInstance(this.ToType());

    public abstract Type ToType();
    public virtual string GetTypeName() => this.ToType().FullName;
}

/// <summary>
/// Creates instances of the Control, simply using Activator.CreateInstance
/// </summary>
public class TypeMappingControlFactory : MappingControlFactory
{
    private readonly Type controlType;

    public TypeMappingControlFactory(string forTypeFullName, string textGlyph, Type controlType)
        : base(forTypeFullName, textGlyph) => this.controlType = controlType;

    public override Type ToType() => this.controlType;
}
