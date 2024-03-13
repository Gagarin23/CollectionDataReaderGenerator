using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace CollectionDataReaderGenerator;

[Generator]
public class CollectionDataReaderIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider
            (
                (s, _) => s is ClassDeclarationSyntax or StructDeclarationSyntax,
                (ctx, _) => GetClassDeclarationForSourceGen(ctx)
            )
            .Where(t => t.ReportAttributeFound)
            .Select((t, _) => t.Item1);

        context.RegisterSourceOutput
        (
            context.CompilationProvider.Combine(provider.Collect()),
            ((ctx, t) => GenerateCode
            (
                ctx,
                t.Left,
                t.Right
            ))
        );
    }

    private static (TypeDeclarationSyntax, bool ReportAttributeFound) GetClassDeclarationForSourceGen(GeneratorSyntaxContext context)
    {
        var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;

        foreach (AttributeListSyntax attributeListSyntax in typeDeclarationSyntax.AttributeLists)
        foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
        {
            if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
            {
                continue;
            }

            string attributeName = attributeSymbol.ContainingType.Name;

            if (attributeName == nameof(GenerateDataReaderAttribute))
            {
                return (typeDeclarationSyntax, true);
            }
        }

        return (typeDeclarationSyntax, false);
    }

    private void GenerateCode
    (
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<TypeDeclarationSyntax> classDeclarations
    )
    {
        foreach (var classDeclarationSyntax in classDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol typeSymbol)
            {
                continue;
            }

            var namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();

            string sourceTypeName;

            if (classDeclarationSyntax.Parent?.SyntaxTree != null
                && compilation.GetSemanticModel(classDeclarationSyntax.Parent?.SyntaxTree).GetDeclaredSymbol(classDeclarationSyntax.Parent) is INamedTypeSymbol parentTypeSymbol)
            {
                sourceTypeName = $"{parentTypeSymbol.Name}.{typeSymbol.Name}";
            }
            else
            {
                sourceTypeName = typeSymbol.Name;
            }

            var targetTypeName = typeSymbol.Name;

            var columnProperties = GetColumnProperties(typeSymbol);
            var sqlTypeInfo = GetSqlTypeInfo(typeSymbol);

            GenerateDataReader(context, namespaceName,
                targetTypeName,
                sourceTypeName,
                columnProperties
            );

            GenerateSqlDataRecordEnumerator(context, namespaceName,
                targetTypeName,
                sourceTypeName,
                columnProperties
            );

            var kind = typeSymbol.TypeKind;

            GeneratePartial(context, namespaceName,
                targetTypeName,
                sqlTypeInfo,
                kind,
                columnProperties
            );
        }
    }

    private SqlTypeInfo GetSqlTypeInfo(INamedTypeSymbol typeSymbol)
    {
        var generateDataReaderAttribute = typeSymbol.GetAttributes().First(x => x.AttributeClass!.Name == nameof(GenerateDataReaderAttribute));
        return new SqlTypeInfo
        {
            SchemaName = generateDataReaderAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == nameof(GenerateDataReaderAttribute.SchemaName))
                .Value.Value as string
            ?? "Generated",
            TypeName = generateDataReaderAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == nameof(GenerateDataReaderAttribute.TypeName))
                .Value.Value as string
            ?? typeSymbol.Name
        };
    }

    private void GenerateSqlDataRecordEnumerator(SourceProductionContext context, string namespaceName, string targetClassName, string sourceClassName, ICollection<ColumnPropertyInfo> columnProperties)
    {
        var propMetadata = GetPropMetadata(columnProperties).ToList();

        var code = $$"""
// <auto-generated/>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace {{namespaceName}}
{
    public class {{targetClassName}}SqlDataRecordIterator : IEnumerable<SqlDataRecord>
    {
        private readonly IEnumerable<{{sourceClassName}}> _source;

        public {{targetClassName}}SqlDataRecordIterator(IEnumerable<{{sourceClassName}}> source)
        {
            _source = source;
        }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var record = new SqlDataRecord
            (
                {{
                    string.Join(",\n                ",
                    propMetadata.Select(x =>
                        $"new SqlMetaData(\"{x.PropName}\", "
                        + $"SqlDbType.{x.SqlDbType}"
                        + $"{
                            x.Lenght switch
                            {
                                -1 => ", " +  "SqlMetaData.Max",
                                0 => string.Empty,
                                _ => ", " +  x.Lenght
                            }
                        }"
                        + $"{(x.Precision == 0 ? string.Empty : ", " + x.Precision)}"
                        + $"{(x.Scale == 0 ? string.Empty : ", " + x.Scale)}"
                        + $")")
                    )
                }}
            );

            foreach (var current in _source)
            {
                {{
                    string.Join("\n                ",
                    propMetadata.Select(x =>
                        $"record.Set{
                            x.SqlDbType switch
                            {
                                SqlDbType.UniqueIdentifier => "Guid",
                                SqlDbType.Bit => "Boolean",
                                SqlDbType.TinyInt => "Byte",
                                SqlDbType.SmallInt => "Int16",
                                SqlDbType.NChar => "Char",
                                SqlDbType.Char => "Char",
                                SqlDbType.Decimal => "Decimal",
                                SqlDbType.Float => "Double",
                                SqlDbType.Real => "Float",
                                SqlDbType.Int => "Int32",
                                SqlDbType.BigInt => "Int64",
                                SqlDbType.NVarChar => "String",
                                SqlDbType.VarChar => "String",
                                SqlDbType.DateTime2 => "DateTime",
                                _ => "Bytes"
        }
                        }({x.Ordinal}, current.{x.PropName});"))
                }}
                yield return record;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<SqlDataRecord>)this).GetEnumerator();
        }
    }
    
    public static class {{targetClassName}}SqlDataRecordIteratorExtensions
    {
        public static List<SqlDataRecord> ToList(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToHashSet(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToArray(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToDictionary(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableList(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableHashSet(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableArray(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableDictionary(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableSortedDictionary(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableSortedSet(this {{targetClassName}}SqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
    }
}
""";

        context.AddSource($"{targetClassName}SqlDataRecordIterator.g.cs", SourceText.From(code, Encoding.UTF8));
    }

    private IEnumerable<(string PropName, SqlDbType SqlDbType, int Ordinal, int Lenght, short Precision, short Scale)> GetPropMetadata(ICollection<ColumnPropertyInfo> columnProperties)
    {
        foreach (var columnProperty in columnProperties)
        {
            var length = columnProperty.Length;
            var precision = columnProperty.NumericPrecision;
            var scale = columnProperty.NumericScale;
            SqlDbType sqlDbType;

            if (columnProperty.SqlTypeName != null)
            {
                var trimmedName = Regex.Replace(columnProperty.SqlTypeName, "[(].*[)]", "");
                ExtractNumericsFromSqlTypeName(columnProperty.SqlTypeName, ref precision, ref scale, ref length);

                sqlDbType = trimmedName switch
                {
                    "uniqueidentifier" => SqlDbType.UniqueIdentifier,
                    "bit" => SqlDbType.Bit,
                    "tinyint" => SqlDbType.TinyInt,
                    "smallint" => SqlDbType.SmallInt,
                    "nchar" => SqlDbType.NChar,
                    "char" => SqlDbType.Char,
                    "decimal" => SqlDbType.Decimal,
                    "float" => SqlDbType.Float,
                    "real" => SqlDbType.Real,
                    "int" => SqlDbType.Int,
                    "bigint" => SqlDbType.BigInt,
                    "nvarchar" => SqlDbType.NVarChar,
                    "varchar" => SqlDbType.VarChar,
                    "datetime2" => SqlDbType.DateTime2,
                    _ => SqlDbType.Binary
                };

                yield return (columnProperty.SourcePropertyName, sqlDbType, columnProperty.Ordinal, length, precision, scale);
                continue;
            }

            var typeSymbol = columnProperty.TypeSymbol;

            if (typeSymbol.ToString() == "System.Guid")
            {
                sqlDbType = SqlDbType.UniqueIdentifier;
            }
            else
            {
                sqlDbType = typeSymbol.SpecialType switch
                {
                    SpecialType.System_Boolean => SqlDbType.Bit,
                    SpecialType.System_Byte => SqlDbType.TinyInt,
                    SpecialType.System_SByte => SqlDbType.SmallInt,
                    SpecialType.System_Char => SqlDbType.NChar,
                    SpecialType.System_Decimal => SqlDbType.Decimal,
                    SpecialType.System_Double => SqlDbType.Float,
                    SpecialType.System_Single => SqlDbType.Real,
                    SpecialType.System_Int32 => SqlDbType.Int,
                    SpecialType.System_Int64 => SqlDbType.BigInt,
                    SpecialType.System_Int16 => SqlDbType.SmallInt,
                    SpecialType.System_String => SqlDbType.NVarChar,
                    SpecialType.System_DateTime => SqlDbType.DateTime2,
                    _ => SqlDbType.Binary
                };
            }

            yield return (columnProperty.SourcePropertyName, sqlDbType, columnProperty.Ordinal, columnProperty.Length, columnProperty.NumericPrecision, columnProperty.NumericScale);
        }
    }

    private void ExtractNumericsFromSqlTypeName(string sqlTypeName, ref short precision, ref short scale, ref int length)
    {
        var extractedNumerics = Regex.Replace
        (
            sqlTypeName,
            "[^0-9,]",
            ""
        );
        if (extractedNumerics.Contains(','))
        {
            var split = extractedNumerics.Split(',');
            precision = short.Parse(split[0]);
            scale = short.Parse(split[1]);
        }
        else
        {
            length = int.Parse(extractedNumerics);
        }
    }

    private void GeneratePartial
    (
        SourceProductionContext context,
        string namespaceName,
        string targetClassName,
        SqlTypeInfo sqlTypeInfo,
        TypeKind kind,
        ICollection<ColumnPropertyInfo> columnProperties
    )
    {
        var sqlColumns = GetColumnNamesAndTypes(columnProperties)
            .Select(x => $"{x.ColumnName} {x.SqlType}")
            .ToList();

        var code = $$"""
// <auto-generated/>
using System.Collections.Generic;

namespace {{namespaceName}}
{
    public partial {{kind switch
{
    TypeKind.Class => "class",
    TypeKind.Struct => "struct",
    _ => throw new ArgumentException($"TypeKind was {kind}")
}}} {{targetClassName}}
    {
        public static {{targetClassName}}DataReader CreateDataReader(IEnumerable<{{targetClassName}}> source)
        {
            return new {{targetClassName}}DataReader(source);
        }

        public static {{targetClassName}}SqlDataRecordIterator CreateSqlDataRecordIterator(IEnumerable<{{targetClassName}}> source)
        {
            return new {{targetClassName}}SqlDataRecordIterator(source);
        }

        public const string CreateTableTypeSqlText = @"
{{CreateTypeSqlText(sqlTypeInfo, sqlColumns)}}
";

        public const string TableTypeName = "{{sqlTypeInfo.SchemaName}}.{{sqlTypeInfo.TypeName}}";

        public const string TempTableName = "#{{sqlTypeInfo.TypeName}}";

        public const string CreateTempTableSqlText = @"
CREATE TABLE #{{sqlTypeInfo.TypeName}} (
    {{string.Join(",\n    ", sqlColumns)}}
);
";
    }
}
""";

        context.AddSource($"{targetClassName}.g.cs", SourceText.From(code, Encoding.UTF8));
    }

    private string CreateTypeSqlText(SqlTypeInfo sqlTypeInfo, List<string> sqlColumns)
    {
        return
            $@"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{sqlTypeInfo.SchemaName}')
BEGIN
    EXEC('CREATE SCHEMA {sqlTypeInfo.SchemaName}');
END;

IF EXISTS (
    SELECT * FROM sys.types
    WHERE is_table_type = 1
    AND name = N'{sqlTypeInfo.TypeName}'
    AND schema_id = SCHEMA_ID(N'{sqlTypeInfo.SchemaName}')
)
BEGIN
    DECLARE @sql NVARCHAR(MAX) = N'DROP TYPE {sqlTypeInfo.SchemaName}.{sqlTypeInfo.TypeName};';
    EXEC sp_executesql @sql;
END;

CREATE TYPE {sqlTypeInfo.SchemaName}.{sqlTypeInfo.TypeName} AS TABLE
(
    {string.Join(",\n    ", sqlColumns)}
);";
    }

    private IEnumerable<(string ColumnName, string SqlType)> GetColumnNamesAndTypes(ICollection<ColumnPropertyInfo> columnProperties)
    {
        foreach (var columnProperty in columnProperties)
        {
            if (columnProperty.SqlTypeName != null)
            {
                yield return (columnProperty.ColumnName, columnProperty.SqlTypeName);
                continue;
            }

            var typeSymbol = columnProperty.TypeSymbol;
            var sqlType = string.Empty;

            if (typeSymbol.ToString() == "System.Guid")
            {
                sqlType = "uniqueidentifier";
            }
            else
            {
                sqlType = typeSymbol.SpecialType switch
                {
                    SpecialType.System_Boolean => "bit",
                    SpecialType.System_Byte => "tinyint",
                    SpecialType.System_SByte => "smallint",
                    SpecialType.System_Char => "nchar(1)",
                    SpecialType.System_Decimal => $"decimal({columnProperty.NumericPrecision}, {columnProperty.NumericScale})",
                    SpecialType.System_Double => "float",
                    SpecialType.System_Single => "real",
                    SpecialType.System_Int32 => "int",
                    SpecialType.System_Int64 => "bigint",
                    SpecialType.System_Int16 => "smallint",
                    SpecialType.System_String => $"nvarchar({(columnProperty.Length == -1 ? "max" : columnProperty.Length.ToString())})",
                    SpecialType.System_DateTime => "datetime2",
                    _ => $"varbinary({(columnProperty.Length == -1 ? "max" : columnProperty.Length.ToString())})"
                };
            }

            yield return (columnProperty.ColumnName, sqlType);
        }
    }

    private void GenerateDataReader(SourceProductionContext context, string namespaceName, string targetClassName, string sourceClassName, ICollection<ColumnPropertyInfo> columnProperties)
    {
            var code = $$"""
// <auto-generated/>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace {{namespaceName}}
{
    public class {{targetClassName}}DataReader : DbDataReader
    {
        private readonly IEnumerator<{{sourceClassName}}> _source;
        private {{sourceClassName}} _current;

        public {{targetClassName}}DataReader(IEnumerable<{{sourceClassName}}> source)
        {
            _source = source.GetEnumerator();
        }

        public override DataTable GetSchemaTable()
        {
            DataTable schemaTable = new DataTable()
            {
                Columns = {
                    {
                        "ColumnOrdinal",
                        typeof (int)
                    },
                    {
                        "ColumnName",
                        typeof (string)
                    },
                    {
                        "DataType",
                        typeof (Type)
                    },
                    {
                        "ColumnSize",
                        typeof (int)
                    },
                    {
                        "AllowDBNull",
                        typeof (bool)
                    },
                    {
                        "IsKey",
                        typeof (bool)
                    },
                    {
                        "NumericPrecision",
                        typeof (short)
                    },
                    {
                        "NumericScale",
                        typeof (short)
                    },
                }
            };

{{GetSchemaTableRows(columnProperties)}}

            return schemaTable;
        }
        
        public override bool GetBoolean(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Boolean))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override byte GetByte(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Byte))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Char))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            var str = GetString(ordinal);
            var val2 = str.Length - (int)dataOffset;
            
            if (val2 <= 0)
            {
                return 0;
            }
            
            int count = Math.Min(length, val2);
            str.CopyTo((int) dataOffset, buffer, bufferOffset, count);
            return (long) count;
        }

        public override string GetDataTypeName(int ordinal)
        {
            return ordinal switch
            {
{{GetDataTypeNameSwitchExpression(columnProperties)}}
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(DateTime))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override decimal GetDecimal(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Decimal))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override double GetDouble(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Double))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override Type GetFieldType(int ordinal)
        {
            return ordinal switch
            {
{{GetFieldTypeSwitchExpression(columnProperties)}}
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override float GetFloat(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Single))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override Guid GetGuid(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Guid))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override short GetInt16(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Int16))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override int GetInt32(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Int32))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override long GetInt64(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(Int64))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override string GetName(int ordinal)
        {
            return ordinal switch
            {
{{GetNameSwitchExpression(columnProperties)}}
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override int GetOrdinal(string name)
        {
            return name switch
            {
{{GetOrdinalSwitchExpression(columnProperties)}}
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
        }

        public override string GetString(int ordinal)
        {
            return ordinal switch
            {
{{GetTypedValueSwitchExpressionByTypeName(columnProperties, nameof(String))}}
                _ => throw new InvalidCastException(),
            };
        }

        public override object GetValue(int ordinal)
        {
            return ordinal switch
            {
{{GetValueSwitchExpression(columnProperties)}}
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            return ordinal switch
            {
{{IsDbNullSwitchExpression(columnProperties)}}
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override int FieldCount => {{columnProperties.Count}};

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override int RecordsAffected => 0;

        public override bool HasRows
        {
            get
            {
                var hasValue = _source.MoveNext();
                _source.Reset();
                return hasValue;
            }
        }

        public override bool IsClosed => false;

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            if (_source.MoveNext())
            {
                _current = _source.Current;
                return true;
            }

            return false;
        }

        public override int Depth => 0;

        public override IEnumerator GetEnumerator()
        {
            return _source;
        }

        public void Reset()
        {
            _source.Reset();
            _current = default;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _source.Dispose();
        }
    }
}
""";

        context.AddSource($"{sourceClassName}DataReader.g.cs", SourceText.From(code, Encoding.UTF8));
    }

    private string GetSchemaTableRows(ICollection<ColumnPropertyInfo> columnProperties)
    {
        return string.Join("\n", columnProperties
            .Select(
                p =>
                {
                    var isNullable = p.TypeSymbol.IsReferenceType || p.TypeSymbol.NullableAnnotation == NullableAnnotation.Annotated;
                    return $"                schemaTable.Rows.Add({p.Ordinal}, \"{p.ColumnName}\", typeof({p.TypeSymbol.Name}), {(p.Length is -1 or 0 ? -1 : p.Length)}, {isNullable.ToString().ToLower()}, false, {p.NumericPrecision}, {p.NumericScale});";
                }));
    }

    private string IsDbNullSwitchExpression(ICollection<ColumnPropertyInfo> columnProperties)
    {
        return string.Join("\n", columnProperties
            .Select(p =>
                {
                    var isNullable = p.TypeSymbol.IsReferenceType || p.TypeSymbol.NullableAnnotation == NullableAnnotation.Annotated;
                    return $"                {p.Ordinal} => {(isNullable ? $"_current.{p.SourcePropertyName} is null" : "false")},";
                }
            ));
    }

    private string GetValueSwitchExpression(ICollection<ColumnPropertyInfo> columnProperties)
    {
        return string.Join("\n", columnProperties
            .Select(p => $"                {p.Ordinal} => _current.{p.SourcePropertyName},"));
    }

    private string GetOrdinalSwitchExpression(ICollection<ColumnPropertyInfo> columnProperties)
    {
        return string.Join("\n", columnProperties
            .Select(p => $"                \"{p.ColumnName}\"{(p.ColumnName != p.SourcePropertyName ? $" or \"{p.SourcePropertyName}\"": "")} => {p.Ordinal},"));
    }

    private string GetNameSwitchExpression(ICollection<ColumnPropertyInfo> columnProperties)
    {
        return string.Join("\n", columnProperties
            .Select(p => $"                {p.Ordinal} => \"{p.ColumnName}\","));
    }

    private string GetFieldTypeSwitchExpression(ICollection<ColumnPropertyInfo> columnProperties)
    {
        return string.Join("\n", columnProperties
            .Select(p => $"                {p.Ordinal} => typeof({p.TypeSymbol.Name}),"));
    }

    private string GetDataTypeNameSwitchExpression(ICollection<ColumnPropertyInfo> columnProperties)
    {
        return string.Join("\n", columnProperties
            .Select(p => $"                {p.Ordinal} => nameof({p.TypeSymbol.Name}),"));
    }

    private string GetTypedValueSwitchExpressionByTypeName(ICollection<ColumnPropertyInfo> columnProperties, string typeName)
    {
        return string.Join("\n", columnProperties
            .Where(p => p.TypeSymbol.Name == typeName)
            .Select(p => $"                {p.Ordinal} => _current.{p.SourcePropertyName},"));
    }

    private ICollection<ColumnPropertyInfo> GetColumnProperties(INamedTypeSymbol classSymbol)
    {
        if (classSymbol == null)
        {
            return Array.Empty<ColumnPropertyInfo>();
        }

        var properties = new List<ColumnPropertyInfo>();

        var props = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.GetAttributes().Any(attr => attr.AttributeClass.Name == nameof(ColumnIgnoreAttribute)))
            .ToList();

        IEnumerable<ColumnPropertyInfo> targetProps;

        if (props.Any(p =>p.GetAttributes().Any(attr => attr.AttributeClass.Name == nameof(ColumnInfoAttribute))))
        {
            var propertyIndex = 0;
            targetProps = props
                .Select
                (
                    p =>
                    {
                        var targetAttr = p
                            .GetAttributes()
                            .FirstOrDefault(attr => attr.AttributeClass.Name == nameof(ColumnInfoAttribute));

                        if (targetAttr == null)
                        {
                            var propInfo = new ColumnPropertyInfo
                            (
                                propertyIndex,
                                p.Name,
                                p.Name,
                                p.Type,
                                (p.Type.Name == nameof(Decimal) ? (short)18 :(short)0),
                                (p.Type.Name == nameof(Decimal) ? (short)5 :(short)0),
                                (p.Type.Name == nameof(String) ? -1 : 0),
                                null
                            );

                            propertyIndex++;
                            return propInfo;
                        }

                        var columnNameProp = targetAttr.NamedArguments
                            .FirstOrDefault(x => x.Key == nameof(ColumnInfoAttribute.ColumnName));

                        var columnName = columnNameProp.Value.Value as string ?? p.Name;

                        var ordinal = targetAttr.NamedArguments
                            .FirstOrDefault(x => x.Key == nameof(ColumnInfoAttribute.Ordinal))
                            .Value.Value as int? ?? propertyIndex;

                        short precision = 0;
                        short scale = 0;
                        int length = 0;

                        var sqlTypeName = targetAttr.NamedArguments
                            .FirstOrDefault(x => x.Key == nameof(ColumnInfoAttribute.SqlTypeName))
                            .Value.Value as string;

                        if (sqlTypeName != null)
                        {
                            ExtractNumericsFromSqlTypeName(sqlTypeName, ref precision, ref scale, ref length);
                        }
                        else
                        {
                            precision = targetAttr.NamedArguments
                                            .FirstOrDefault(x => x.Key == nameof(ColumnInfoAttribute.NumericPrecision))
                                            .Value.Value as short?
                                        ?? (p.Type.Name == nameof(Decimal) ? (short)18 : (short)0);

                            scale = targetAttr.NamedArguments
                                        .FirstOrDefault(x => x.Key == nameof(ColumnInfoAttribute.NumericScale))
                                        .Value.Value as short?
                                    ?? (p.Type.Name == nameof(Decimal) ? (short)5 : (short)0);

                            length = targetAttr.NamedArguments
                                         .FirstOrDefault(x => x.Key == nameof(ColumnInfoAttribute.Length))
                                         .Value.Value as int?
                                     ?? (p.Type.Name == nameof(String) ? -1 : 0);
                        }

                        propertyIndex++;

                        return new ColumnPropertyInfo(ordinal, columnName, p.Name, p.Type, precision, scale, length, sqlTypeName);
                    }
                )
                .OrderBy(p => p.Ordinal);
        }
        else
        {
            var propertyIndex = 0;
            targetProps = props
                .Select
                (
                    p =>
                    {
                        var propInfo = new ColumnPropertyInfo
                        (
                            propertyIndex,
                            p.Name,
                            p.Name,
                            p.Type,
                            (p.Type.Name == nameof(Decimal) ?(short)18 :(short)0),
                            (p.Type.Name == nameof(Decimal) ?(short)5 :(short)0),
                            (p.Type.Name == nameof(String) ? -1 : 0),
                            null
                        );

                        propertyIndex++;
                        return propInfo;
                    }
                )
                .OrderBy(p => p.Ordinal);
        }

        properties.AddRange(targetProps);

        return properties;
    }
}
