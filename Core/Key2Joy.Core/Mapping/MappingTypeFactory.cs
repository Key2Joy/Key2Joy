using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Actions.Scripting;

namespace Key2Joy.Mapping;

/// <summary>
/// Creates instances of types, simply using Activator.CreateInstance
/// </summary>
public class MappingTypeFactory
{
    public string FullTypeName { get; private set; }

    public MappingAttribute Attribute { get; private set; }
    public IEnumerable<ExposedMethod> ExposedMethods { get; private set; }

    public MappingTypeFactory(string fullTypeName, MappingAttribute attribute, IEnumerable<ExposedMethod> exposedMethods = null)
    {
        this.FullTypeName = fullTypeName;
        this.Attribute = attribute;
        this.ExposedMethods = exposedMethods ?? new List<ExposedMethod>();
    }

    public virtual T CreateInstance<T>(object[] constructorArguments) where T : AbstractMappingAspect => (T)Activator.CreateInstance(this.ToType(), constructorArguments);

    public virtual Type ToType() => Type.GetType(this.FullTypeName);
}

/// <summary>
/// Creates instances of types, simply using Activator.CreateInstance
/// </summary>
public class MappingTypeFactory<T> : MappingTypeFactory where T : AbstractMappingAspect
{
    public MappingTypeFactory(string fullTypeName, MappingAttribute attribute, IEnumerable<ExposedMethod> exposedMethods = null)
        : base(fullTypeName, attribute, exposedMethods)
    {
    }

    public virtual T CreateInstance(object[] constructorArguments) => base.CreateInstance<T>(constructorArguments);
}
