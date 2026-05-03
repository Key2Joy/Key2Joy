using System;

namespace Key2Joy.Contracts.Mapping;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MappingControlAttribute : Attribute
{
    public Type ForType { get; set; }
    public Type[] ForTypes { get; set; }
    public string TextGlyph { get; set; } = "\uE711";
}
