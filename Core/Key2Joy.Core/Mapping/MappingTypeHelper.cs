using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Key2Joy.Contracts.Mapping;

namespace Key2Joy.Mapping;

public class MappingTypeHelper
{
    /// <summary>
    /// Ensures the typename is valid, splitting the long variant into its short form
    /// </summary>
    /// <param name="typeInfoTypeName"></param>
    /// <returns></returns>
    public static string EnsureSimpleTypeName(string typeInfoTypeName) => typeInfoTypeName.Split(',')[0];

    /// <summary>
    /// Gets the typename, even if the object is a proxy
    /// </summary>
    /// <param name="typeFactories"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetTypeFullName<T>(IDictionary<string, MappingTypeFactory<T>> typeFactories, AbstractMappingAspect instance)
        where T : AbstractMappingAspect
    {
        var typeName = instance.GetType().FullName;

        if (!typeFactories.ContainsKey(typeName))
        {
            throw new ArgumentException("Only allowed types may be (de)serialized");
        }

        return typeName;
    }

    /// <summary>
    /// Gets the typename, even if the object is a proxy
    /// </summary>
    /// <param name="typeFactories"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetTypeFullName(IDictionary<string, MappingTypeFactory> typeFactories, AbstractMappingAspect instance)
    {
        var typeName = instance.GetType().FullName;

        if (!typeFactories.ContainsKey(typeName))
        {
            throw new ArgumentException("Only allowed types may be (de)serialized");
        }

        return typeName;
    }
}
