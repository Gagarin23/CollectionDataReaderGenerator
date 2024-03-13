using System;

namespace CollectionDataReaderGenerator;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class GenerateDataReaderAttribute : Attribute
{
    public string SchemaName { get; set; }
    public string TypeName { get; set; }
}
