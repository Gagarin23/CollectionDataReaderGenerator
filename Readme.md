# Collection DataReader Generator

This is a very fast and tiny tool to generate a DataReader for a collection of objects. Attributes do collection to sql table parameter mapping.
- Boxing free
- No reflection
- No IL generation
- Fastest reading by switch case statement

Nuget: https://www.nuget.org/packages/DotnetMinistry.CollectionDataReaderGenerator


## Usage
One simple use case is set collection like sql stored procedure table parameter.

Example:
<details>
  <summary>Target class</summary>

```csharp
    using CollectionDataReaderGenerator;
    
    namespace CollectionToStoredProc;
    
    [DataReader]
    public class PrimitiveTypesTableType
    {
        public Guid GuidColumn { get; set; }
        [ColumnInfo(Ordinal = 1)]public bool BooleanColumn { get; set; }
        [ColumnInfo(ColumnName = "StringColumn")]public string Qwerty { get; set; }
        public decimal DecimalColumn { get; set; }
        public double DoubleColumn { get; set; }
        public float FloatColumn { get; set; }
        public float FloatColumn_2 { get; set; }
        public int IntColumn { get; set; }
        public long LongColumn { get; set; }
        public short ShortColumn { get; set; }
        public DateTime DateTimeColumn { get; set; }
        [ColumnIgnore] public Object Obj { get; set; }
    }
```
</details>

<details>
  <summary>Generated</summary>

```csharp
    // <auto-generated/>
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    
    namespace CollectionToStoredProc;
    
    class PrimitiveTypesTableTypeDataReader : DbDataReader
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
                schemaTable.Rows.Add(3, "DecimalColumn", typeof(Decimal), -1, false, false, 18, 5);
                schemaTable.Rows.Add(4, "DoubleColumn", typeof(Double), -1, false, false, 0, 0);
                schemaTable.Rows.Add(5, "FloatColumn", typeof(Single), -1, false, false, 0, 0);
                schemaTable.Rows.Add(6, "FloatColumn_2", typeof(Single), -1, false, false, 0, 0);
                schemaTable.Rows.Add(7, "IntColumn", typeof(Int32), -1, false, false, 0, 0);
                schemaTable.Rows.Add(8, "LongColumn", typeof(Int64), -1, false, false, 0, 0);
                schemaTable.Rows.Add(9, "ShortColumn", typeof(Int16), -1, false, false, 0, 0);
                schemaTable.Rows.Add(10, "DateTimeColumn", typeof(DateTime), -1, false, false, 0, 0);
    
            return schemaTable;
        }
        
        public override bool GetBoolean(int ordinal)
        {
            return ordinal switch
            {
                1 => _current.BooleanColumn,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override byte GetByte(int ordinal)
        {
            return ordinal switch
            {
    
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
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
    
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
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
                3 => nameof(Decimal),
                4 => nameof(Double),
                5 => nameof(Single),
                6 => nameof(Single),
                7 => nameof(Int32),
                8 => nameof(Int64),
                9 => nameof(Int16),
                10 => nameof(DateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }
    
        public override DateTime GetDateTime(int ordinal)
        {
            return ordinal switch
            {
                10 => _current.DateTimeColumn,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override decimal GetDecimal(int ordinal)
        {
            return ordinal switch
            {
                3 => _current.DecimalColumn,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override double GetDouble(int ordinal)
        {
            return ordinal switch
            {
                4 => _current.DoubleColumn,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override Type GetFieldType(int ordinal)
        {
            return ordinal switch
            {
                0 => typeof(Guid),
                1 => typeof(Boolean),
                2 => typeof(String),
                3 => typeof(Decimal),
                4 => typeof(Double),
                5 => typeof(Single),
                6 => typeof(Single),
                7 => typeof(Int32),
                8 => typeof(Int64),
                9 => typeof(Int16),
                10 => typeof(DateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }
    
        public override float GetFloat(int ordinal)
        {
            return ordinal switch
            {
                5 => _current.FloatColumn,
                6 => _current.FloatColumn_2,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override Guid GetGuid(int ordinal)
        {
            return ordinal switch
            {
                0 => _current.GuidColumn,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override short GetInt16(int ordinal)
        {
            return ordinal switch
            {
                9 => _current.ShortColumn,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override int GetInt32(int ordinal)
        {
            return ordinal switch
            {
                7 => _current.IntColumn,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override long GetInt64(int ordinal)
        {
            return ordinal switch
            {
                8 => _current.LongColumn,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override string GetName(int ordinal)
        {
            return ordinal switch
            {
                0 => "GuidColumn",
                1 => "BooleanColumn",
                2 => "StringColumn",
                3 => "DecimalColumn",
                4 => "DoubleColumn",
                5 => "FloatColumn",
                6 => "FloatColumn_2",
                7 => "IntColumn",
                8 => "LongColumn",
                9 => "ShortColumn",
                10 => "DateTimeColumn",
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }
    
        public override int GetOrdinal(string name)
        {
            return name switch
            {
                "GuidColumn" => 0,
                "BooleanColumn" => 1,
                "StringColumn" or "Qwerty" => 2,
                "DecimalColumn" => 3,
                "DoubleColumn" => 4,
                "FloatColumn" => 5,
                "FloatColumn_2" => 6,
                "IntColumn" => 7,
                "LongColumn" => 8,
                "ShortColumn" => 9,
                "DateTimeColumn" => 10,
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
        }
    
        public override string GetString(int ordinal)
        {
            return ordinal switch
            {
                2 => _current.Qwerty,
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(ordinal)),
                _ => throw new InvalidCastException()
            };
        }
    
        public override object GetValue(int ordinal)
        {
            return ordinal switch
            {
                0 => _current.GuidColumn,
                1 => _current.BooleanColumn,
                2 => _current.Qwerty,
                3 => _current.DecimalColumn,
                4 => _current.DoubleColumn,
                5 => _current.FloatColumn,
                6 => _current.FloatColumn_2,
                7 => _current.IntColumn,
                8 => _current.LongColumn,
                9 => _current.ShortColumn,
                10 => _current.DateTimeColumn,
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
                2 => _current.Qwerty is null,
                3 => false,
                4 => false,
                5 => false,
                6 => false,
                7 => false,
                8 => false,
                9 => false,
                10 => false,
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }
    
        public override int FieldCount => 11;
    
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
    
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _source.Dispose();
        }
    }
```

</details>

<details>
  <summary>Code for testing</summary>

```csharp
    using System.Data;
    using CollectionToStoredProc;
    using Microsoft.Data.SqlClient;
    
    var data = Enumerable.Range(0, 100)
        .Select
        (
            x => new PrimitiveTypesTableType
            {
                GuidColumn = Guid.NewGuid(),
                BooleanColumn = x % 2 == 0,
                Qwerty = x.ToString(),
                DecimalColumn = x + 0.123m,
                DoubleColumn = x + 0.123,
                FloatColumn = x + 0.123f,
                IntColumn = x,
                LongColumn = x,
                ShortColumn = (short)x,
                DateTimeColumn = DateTime.Now.AddDays(x)
            }
        )
        .ToList();
    
    await using var sqlConnection = new SqlConnection("YourConnectionString");
    
    await using var createTableAndTypeCmd = new SqlCommand
    {
        Connection = sqlConnection,
        CommandType = CommandType.Text,
        CommandText = @"
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PrimitiveTypesTable')
        CREATE TABLE PrimitiveTypesTable
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            GuidColumn UNIQUEIDENTIFIER,
            BooleanColumn BIT,
            StringColumn NVARCHAR(100),
            DecimalColumn DECIMAL(18, 5),
            DoubleColumn FLOAT,
            FloatColumn REAL,
            FloatColumn_2 REAL,
            IntColumn INT,
            LongColumn BIGINT,
            ShortColumn SMALLINT,
            DateTimeColumn DATETIME
        )
    
    IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'PrimitiveTypesTableType')
        CREATE TYPE PrimitiveTypesTableType AS TABLE
        (
            GuidColumn UNIQUEIDENTIFIER,
            BooleanColumn BIT,
            StringColumn NVARCHAR(100),
            DecimalColumn DECIMAL(18, 5),
            DoubleColumn FLOAT,
            FloatColumn REAL,
            FloatColumn_2 REAL,
            IntColumn INT,
            LongColumn BIGINT,
            ShortColumn SMALLINT,
            DateTimeColumn DATETIME
        )
    "
    };
    await using var createSpCmd = new SqlCommand
    {
        Connection = sqlConnection,
        CommandType = CommandType.Text,
        CommandText = @"
    CREATE or ALTER PROCEDURE [dbo].[sp_InsertPrimitiveTypesTable]
        @Data PrimitiveTypesTableType READONLY
    AS
    BEGIN
        INSERT INTO PrimitiveTypesTable(GuidColumn, BooleanColumn, StringColumn, DecimalColumn, DoubleColumn, FloatColumn, FloatColumn_2, IntColumn, LongColumn, ShortColumn, DateTimeColumn)
        SELECT * FROM @Data
    END
    "
    };
    
    await using var callSpCmd = new SqlCommand
    {
        Connection = sqlConnection,
        CommandType = CommandType.StoredProcedure,
        CommandText = "sp_InsertPrimitiveTypesTable"
    };
    
    callSpCmd.Parameters.Add(new SqlParameter
    {
        ParameterName = "Data",
        SqlDbType = SqlDbType.Structured,
        Direction = ParameterDirection.Input,
        Value = new PrimitiveTypesTableTypeDataReader(data)
    });
    
    await using var selectCmd = new SqlCommand
    {
        Connection = sqlConnection,
        CommandType = CommandType.Text,
        CommandText = @"
    select top 1
        *
    from PrimitiveTypesTable
    order by Id desc;"
    };
    
    await sqlConnection.OpenAsync();
    try
    {
        createTableAndTypeCmd.ExecuteNonQuery();
        createSpCmd.ExecuteNonQuery();
        callSpCmd.ExecuteNonQuery();
        var reader = await selectCmd.ExecuteReaderAsync();
    
        while (await reader.ReadAsync())
        {
            var id = reader.GetInt32(0);
            var guidColumn = reader.GetGuid(1);
            var booleanColumn = reader.GetBoolean(2);
            var stringColumn = reader.GetString(3);
            var decimalColumn = reader.GetDecimal(4);
            var doubleColumn = reader.GetDouble(5);
            var floatColumn = reader.GetFloat(6);
            var floatColumn_2 = reader.GetFloat(7);
            var intColumn = reader.GetInt32(8);
            var longColumn = reader.GetInt64(9);
            var shortColumn = reader.GetInt16(10);
            var dateTimeColumn = reader.GetDateTime(11);
        }
    }
    finally
    {
        await sqlConnection.CloseAsync();
    }
```

</details>
