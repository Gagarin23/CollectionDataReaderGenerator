using Microsoft.CodeAnalysis;

namespace CollectionDataReaderGenerator;

public class ColumnPropertyInfo
{
    public int Ordinal { get; }
    public string ColumnName { get; }
    public string SourcePropertyName { get; }
    public ITypeSymbol TypeSymbol { get; }
    public short NumericPrecision { get; }
    public short NumericScale { get; }

    public ColumnPropertyInfo(int ordinal, string columnName, string sourcePropertyName, ITypeSymbol typeSymbol, short numericPrecision, short numericScale)
    {
        Ordinal = ordinal;
        ColumnName = columnName;
        SourcePropertyName = sourcePropertyName;
        TypeSymbol = typeSymbol;
        NumericPrecision = numericPrecision;
        NumericScale = numericScale;
    }
}
