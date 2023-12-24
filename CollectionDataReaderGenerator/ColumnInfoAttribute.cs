using System;

namespace CollectionDataReaderGenerator;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnInfoAttribute : Attribute
{
    public int Ordinal { get; set; }
    public string? ColumnName { get; set; }
    public short NumericPrecision { get; set; }
    public short NumericScale { get; set; }
    public int Length { get; set; }
    public string? SqlType { get; set; }
}
