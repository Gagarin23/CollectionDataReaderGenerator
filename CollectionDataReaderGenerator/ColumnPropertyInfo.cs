using Microsoft.CodeAnalysis;

namespace CollectionDataReaderGenerator;

internal class ColumnPropertyInfo
{
    public int Ordinal { get; }
    public string ColumnName { get; }
    public string SourcePropertyName { get; }
    public ITypeSymbol TypeSymbol { get; }
    public short NumericPrecision { get; }
    public short NumericScale { get; }
    public int Length { get; }
    public string? SqlType { get; }

    public ColumnPropertyInfo
    (
        int ordinal,
        string columnName,
        string sourcePropertyName,
        ITypeSymbol typeSymbol,
        short numericPrecision,
        short numericScale,
        int length,
        string? sqlType
    )
    {
        Ordinal = ordinal;
        ColumnName = columnName;
        SourcePropertyName = sourcePropertyName;
        TypeSymbol = typeSymbol;
        NumericPrecision = numericPrecision;
        NumericScale = numericScale;
        Length = length;
        SqlType = sqlType;
    }
}
