using System;

namespace CollectionDataReaderGenerator;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public int Ordinal { get; set; }
    public string? ColumnName { get; set; }
    public short NumericPrecision { get; set; }
    public short NumericScale { get; set; }

    public ColumnAttribute(int ordinal)
    {
        Ordinal = ordinal;
    }
}
