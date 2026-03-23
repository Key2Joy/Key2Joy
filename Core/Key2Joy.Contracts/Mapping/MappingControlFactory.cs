using System;
using System.Windows.Forms;

namespace Key2Joy.Contracts.Mapping;

/// <summary>
/// Creates instances of the Control, simply using Activator.CreateInstance
/// </summary>
public abstract class MappingControlFactory
{
    public string ForTypeFullName { get; private set; }
    public string ImageResourceName { get; private set; }

    public MappingControlFactory(string forTypeFullName, string imageResourceName)
    {
        this.ForTypeFullName = forTypeFullName;
        this.ImageResourceName = imageResourceName;
    }

    public virtual T CreateInstance<T>() where T : Control => (T)Activator.CreateInstance(this.ToType());

    public abstract Type ToType();
    public virtual string GetTypeName() => this.ToType().FullName;
}

/// <summary>
/// Creates instances of the Control, simply using Activator.CreateInstance
/// </summary>
public class TypeMappingControlFactory : MappingControlFactory
{
    private readonly Type controlType;

    public TypeMappingControlFactory(string forTypeFullName, string imageResourceName, Type controlType)
        : base(forTypeFullName, imageResourceName) => this.controlType = controlType;

    public override Type ToType() => this.controlType;
}
