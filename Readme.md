# Collection DataReader Generator & SqlDataRecord Iterator

This is a very fast and tiny tool to generate a DataReader and IEnumerable\<SqlDataRecord\> for a collection of objects. Attributes do collection to sql table parameter mapping.
- Boxing free
- No reflection
- No IL generation
- Fastest reading by switch case statement

Nuget: https://www.nuget.org/packages/DotnetMinistry.CollectionDataReaderGenerator

Mssql driver level benchmark:\
![image](https://github.com/Gagarin23/CollectionDataReaderGenerator/assets/59282770/77d4d2d8-9487-4c0c-ad5e-2938341fc0e2)


## Usage
One simple use case is set collection like sql stored procedure table parameter.

Example:
<details>
  <summary>Target class</summary>

```csharp
using CollectionDataReaderGenerator;

namespace CollectionToStoredProc.Models
{
    [GenerateDataReader]
    public partial struct PrimitiveTypesTableType
    {
        public Guid GuidColumn { get; set; }
        
        public bool BooleanColumn { get; set; }
        
        public string StringColumn { get; set; }

        [ColumnInfo(SqlTypeName = "varchar(10)")]
        public string StringColumn2 { get; set; }

        [ColumnInfo(Length = 20, Ordinal = 4)]
        public string StringColumn3 { get; set; }
        
        public decimal DecimalColumn { get; set; }
        
        public double DoubleColumn { get; set; }
        
        public float FloatColumn { get; set; }
        
        public int IntColumn { get; set; }
        
        public long LongColumn { get; set; }
        
        public short ShortColumn { get; set; }
        
        public DateTime DateTimeColumn { get; set; }

        [ColumnIgnore]
        public object Obj { get; set; }
    }
}
```
</details>

<details>
  <summary>Generated</summary>
<details>
    <summary>Partial</summary>

```csharp
// <auto-generated/>
using System.Collections.Generic;

namespace CollectionToStoredProc.Models
{
    public partial struct PrimitiveTypesTableType
    {
        public static PrimitiveTypesTableTypeDataReader CreateDataReader(IEnumerable<PrimitiveTypesTableType> source)
        {
            return new PrimitiveTypesTableTypeDataReader(source);
        }

        public static PrimitiveTypesTableTypeSqlDataRecordIterator CreateSqlDataRecordIterator(IEnumerable<PrimitiveTypesTableType> source)
        {
            return new PrimitiveTypesTableTypeSqlDataRecordIterator(source);
        }

        public const string CreateTableTypeSqlText = @"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Generated')
BEGIN
    EXEC('CREATE SCHEMA Generated');
END;

IF EXISTS (
    SELECT * FROM sys.types
    WHERE is_table_type = 1
    AND name = N'PrimitiveTypesTableType'
    AND schema_id = SCHEMA_ID(N'Generated')
)
BEGIN
    DECLARE @sql NVARCHAR(MAX) = N'DROP TYPE Generated.PrimitiveTypesTableType;';
    EXEC sp_executesql @sql;
END;

CREATE TYPE Generated.PrimitiveTypesTableType AS TABLE
(
    GuidColumn uniqueidentifier,
    BooleanColumn bit,
    StringColumn nvarchar(max),
    StringColumn2 varchar(10),
    StringColumn3 nvarchar(20),
    DecimalColumn decimal(18, 5),
    DoubleColumn float,
    FloatColumn real,
    IntColumn int,
    LongColumn bigint,
    ShortColumn smallint,
    DateTimeColumn datetime2
);
";

        public const string TableTypeName = "Generated.PrimitiveTypesTableType";

        public const string TempTableName = "#PrimitiveTypesTableType";

        public const string CreateTempTableSqlText = @"
CREATE TABLE #PrimitiveTypesTableType (
    GuidColumn uniqueidentifier,
    BooleanColumn bit,
    StringColumn nvarchar(max),
    StringColumn2 varchar(10),
    StringColumn3 nvarchar(20),
    DecimalColumn decimal(18, 5),
    DoubleColumn float,
    FloatColumn real,
    IntColumn int,
    LongColumn bigint,
    ShortColumn smallint,
    DateTimeColumn datetime2
);
";
    }
}
```
</details>

<details>
    <summary>DataReader</summary>

```csharp
// <auto-generated/>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace CollectionToStoredProc.Models
{
    public class PrimitiveTypesTableTypeDataReader : DbDataReader
    {
        private readonly IEnumerator<PrimitiveTypesTableType> _source;
        private PrimitiveTypesTableType _current;

        public PrimitiveTypesTableTypeDataReader(IEnumerable<PrimitiveTypesTableType> source)
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

                schemaTable.Rows.Add(0, "GuidColumn", typeof(Guid), -1, false, false, 0, 0);
                schemaTable.Rows.Add(1, "BooleanColumn", typeof(Boolean), -1, false, false, 0, 0);
                schemaTable.Rows.Add(2, "StringColumn", typeof(String), -1, true, false, 0, 0);
                schemaTable.Rows.Add(3, "StringColumn2", typeof(String), 10, true, false, 0, 0);
                schemaTable.Rows.Add(4, "StringColumn3", typeof(String), 20, true, false, 0, 0);
                schemaTable.Rows.Add(5, "DecimalColumn", typeof(Decimal), -1, false, false, 18, 5);
                schemaTable.Rows.Add(6, "DoubleColumn", typeof(Double), -1, false, false, 0, 0);
                schemaTable.Rows.Add(7, "FloatColumn", typeof(Single), -1, false, false, 0, 0);
                schemaTable.Rows.Add(8, "IntColumn", typeof(Int32), -1, false, false, 0, 0);
                schemaTable.Rows.Add(9, "LongColumn", typeof(Int64), -1, false, false, 0, 0);
                schemaTable.Rows.Add(10, "ShortColumn", typeof(Int16), -1, false, false, 0, 0);
                schemaTable.Rows.Add(11, "DateTimeColumn", typeof(DateTime), -1, false, false, 0, 0);

            return schemaTable;
        }
        
        public override bool GetBoolean(int ordinal)
        {
            return ordinal switch
            {
                1 => _current.BooleanColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override byte GetByte(int ordinal)
        {
            return ordinal switch
            {

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
                0 => nameof(Guid),
                1 => nameof(Boolean),
                2 => nameof(String),
                3 => nameof(String),
                4 => nameof(String),
                5 => nameof(Decimal),
                6 => nameof(Double),
                7 => nameof(Single),
                8 => nameof(Int32),
                9 => nameof(Int64),
                10 => nameof(Int16),
                11 => nameof(DateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return ordinal switch
            {
                11 => _current.DateTimeColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override decimal GetDecimal(int ordinal)
        {
            return ordinal switch
            {
                5 => _current.DecimalColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override double GetDouble(int ordinal)
        {
            return ordinal switch
            {
                6 => _current.DoubleColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override Type GetFieldType(int ordinal)
        {
            return ordinal switch
            {
                0 => typeof(Guid),
                1 => typeof(Boolean),
                2 => typeof(String),
                3 => typeof(String),
                4 => typeof(String),
                5 => typeof(Decimal),
                6 => typeof(Double),
                7 => typeof(Single),
                8 => typeof(Int32),
                9 => typeof(Int64),
                10 => typeof(Int16),
                11 => typeof(DateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override float GetFloat(int ordinal)
        {
            return ordinal switch
            {
                7 => _current.FloatColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override Guid GetGuid(int ordinal)
        {
            return ordinal switch
            {
                0 => _current.GuidColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override short GetInt16(int ordinal)
        {
            return ordinal switch
            {
                10 => _current.ShortColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override int GetInt32(int ordinal)
        {
            return ordinal switch
            {
                8 => _current.IntColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override long GetInt64(int ordinal)
        {
            return ordinal switch
            {
                9 => _current.LongColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override string GetName(int ordinal)
        {
            return ordinal switch
            {
                0 => "GuidColumn",
                1 => "BooleanColumn",
                2 => "StringColumn",
                3 => "StringColumn2",
                4 => "StringColumn3",
                5 => "DecimalColumn",
                6 => "DoubleColumn",
                7 => "FloatColumn",
                8 => "IntColumn",
                9 => "LongColumn",
                10 => "ShortColumn",
                11 => "DateTimeColumn",
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override int GetOrdinal(string name)
        {
            return name switch
            {
                "GuidColumn" => 0,
                "BooleanColumn" => 1,
                "StringColumn" => 2,
                "StringColumn2" => 3,
                "StringColumn3" => 4,
                "DecimalColumn" => 5,
                "DoubleColumn" => 6,
                "FloatColumn" => 7,
                "IntColumn" => 8,
                "LongColumn" => 9,
                "ShortColumn" => 10,
                "DateTimeColumn" => 11,
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
        }

        public override string GetString(int ordinal)
        {
            return ordinal switch
            {
                2 => _current.StringColumn,
                3 => _current.StringColumn2,
                4 => _current.StringColumn3,
                _ => throw new InvalidCastException(),
            };
        }

        public override object GetValue(int ordinal)
        {
            return ordinal switch
            {
                0 => _current.GuidColumn,
                1 => _current.BooleanColumn,
                2 => _current.StringColumn,
                3 => _current.StringColumn2,
                4 => _current.StringColumn3,
                5 => _current.DecimalColumn,
                6 => _current.DoubleColumn,
                7 => _current.FloatColumn,
                8 => _current.IntColumn,
                9 => _current.LongColumn,
                10 => _current.ShortColumn,
                11 => _current.DateTimeColumn,
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
                0 => false,
                1 => false,
                2 => _current.StringColumn is null,
                3 => _current.StringColumn2 is null,
                4 => _current.StringColumn3 is null,
                5 => false,
                6 => false,
                7 => false,
                8 => false,
                9 => false,
                10 => false,
                11 => false,
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override int FieldCount => 12;

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
```

</details>

<details>
    <summary>SqlDataRecordIterator</summary>

```csharp
// <auto-generated/>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace CollectionToStoredProc.Models
{
    public class PrimitiveTypesTableTypeSqlDataRecordIterator : IEnumerable<SqlDataRecord>
    {
        private readonly IEnumerable<PrimitiveTypesTableType> _source;

        public PrimitiveTypesTableTypeSqlDataRecordIterator(IEnumerable<PrimitiveTypesTableType> source)
        {
            _source = source;
        }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var record = new SqlDataRecord
            (
                new SqlMetaData("GuidColumn", SqlDbType.UniqueIdentifier),
                new SqlMetaData("BooleanColumn", SqlDbType.Bit),
                new SqlMetaData("StringColumn", SqlDbType.NVarChar, SqlMetaData.Max),
                new SqlMetaData("StringColumn2", SqlDbType.VarChar, 10),
                new SqlMetaData("StringColumn3", SqlDbType.NVarChar, 20),
                new SqlMetaData("DecimalColumn", SqlDbType.Decimal, 18, 5),
                new SqlMetaData("DoubleColumn", SqlDbType.Float),
                new SqlMetaData("FloatColumn", SqlDbType.Real),
                new SqlMetaData("IntColumn", SqlDbType.Int),
                new SqlMetaData("LongColumn", SqlDbType.BigInt),
                new SqlMetaData("ShortColumn", SqlDbType.SmallInt),
                new SqlMetaData("DateTimeColumn", SqlDbType.DateTime2)
            );

            foreach (var current in _source)
            {
                record.SetGuid(0, current.GuidColumn);
                record.SetBoolean(1, current.BooleanColumn);
                record.SetString(2, current.StringColumn);
                record.SetString(3, current.StringColumn2);
                record.SetString(4, current.StringColumn3);
                record.SetDecimal(5, current.DecimalColumn);
                record.SetDouble(6, current.DoubleColumn);
                record.SetFloat(7, current.FloatColumn);
                record.SetInt32(8, current.IntColumn);
                record.SetInt64(9, current.LongColumn);
                record.SetInt16(10, current.ShortColumn);
                record.SetDateTime(11, current.DateTimeColumn);
                yield return record;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<SqlDataRecord>)this).GetEnumerator();
        }
    }
    
    public static class PrimitiveTypesTableTypeSqlDataRecordIteratorExtensions
    {
        public static List<SqlDataRecord> ToList(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToHashSet(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToArray(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToDictionary(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableList(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableHashSet(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableArray(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableDictionary(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableSortedDictionary(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
        
        public static List<SqlDataRecord> ToImmutableSortedSet(this PrimitiveTypesTableTypeSqlDataRecordIterator source)
        {
            throw new NotImplementedException();
        }
    }
}
```

</details>

</details>

<details>
  <summary>Code for testing</summary>

```csharp
using System.Data;
using CollectionToStoredProc.Models;
using Microsoft.Data.SqlClient;

// iterator with no allocation
var iterator = Enumerable.Range(0, 100)
    .Select
    (
        x => new PrimitiveTypesTableType
        {
            GuidColumn = Guid.NewGuid(),
            BooleanColumn = x % 2 == 0,
            StringColumn = x.ToString(),
            StringColumn2 = x.ToString(),
            StringColumn3 = x.ToString(),
            DecimalColumn = x + 0.123m,
            DoubleColumn = x + 0.123,
            FloatColumn = x + 0.123f,
            IntColumn = x,
            LongColumn = x,
            ShortColumn = (short)x,
            DateTimeColumn = DateTime.Now.AddDays(x)
        }
    );

await using var sqlConnection = new SqlConnection("Server=.;Database=TestDb;Integrated Security=false;TrustServerCertificate=true;User Id=sa;Password=xixikalkaN23");

await using var createTableAndTypeCmd = new SqlCommand
{
    Connection = sqlConnection,
    CommandType = CommandType.Text,
    CommandText = PrimitiveTypesTableType.CreateTableTypeSqlText
};

await using var selectByReaderCmd = new SqlCommand
{
    Connection = sqlConnection,
    CommandType = CommandType.Text,
    CommandText = "SELECT * FROM @Data"
};

selectByReaderCmd.Parameters.Add(new SqlParameter
{
    ParameterName = "Data",
    SqlDbType = SqlDbType.Structured,
    Direction = ParameterDirection.Input,
    TypeName = PrimitiveTypesTableType.TableTypeName,
    Value = PrimitiveTypesTableType.CreateDataReader(iterator)
});

await using var selectByRecordCmd = new SqlCommand
{
    Connection = sqlConnection,
    CommandType = CommandType.Text,
    CommandText = "SELECT * FROM @Data"
};

selectByRecordCmd.Parameters.Add(new SqlParameter
{
    ParameterName = "Data",
    SqlDbType = SqlDbType.Structured,
    Direction = ParameterDirection.Input,
    TypeName = PrimitiveTypesTableType.TableTypeName,
    Value = PrimitiveTypesTableType.CreateSqlDataRecordIterator(iterator)
});

await sqlConnection.OpenAsync();
try
{
    createTableAndTypeCmd.ExecuteNonQuery();

    await using (var reader = await selectByReaderCmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            ReadResult(reader);
        }
    }

    await using (var reader2 = await selectByRecordCmd.ExecuteReaderAsync())
    {
        while (await reader2.ReadAsync())
        {
            ReadResult(reader2);
        }
    }
}
finally
{
    await sqlConnection.CloseAsync();
}

return;

void ReadResult(SqlDataReader sqlDataReader)
{
    var guidColumn = sqlDataReader.GetGuid(0);
    var booleanColumn = sqlDataReader.GetBoolean(1);
    var stringColumn = sqlDataReader.GetString(2);
    var stringColumn2 = sqlDataReader.GetString(3);
    var stringColumn3 = sqlDataReader.GetString(4);
    var decimalColumn = sqlDataReader.GetDecimal(5);
    var doubleColumn = sqlDataReader.GetDouble(6);
    var floatColumn = sqlDataReader.GetFloat(7);
    var intColumn = sqlDataReader.GetInt32(8);
    var longColumn = sqlDataReader.GetInt64(9);
    var shortColumn = sqlDataReader.GetInt16(10);
    var dateTimeColumn = sqlDataReader.GetDateTime(11);
}
```

</details>
