﻿// <auto-generated/>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace CollectionDataReaderGenerator.Tests.TestData
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

                schemaTable.Rows.Add(0, "Qwerty", typeof(Guid), -1, false, false, 0, 0);
                schemaTable.Rows.Add(1, "BooleanColumn", typeof(Boolean), -1, false, false, 0, 0);
                schemaTable.Rows.Add(2, "StringColumn", typeof(String), -1, true, false, 0, 0);
                schemaTable.Rows.Add(3, "DecimalColumn", typeof(Decimal), -1, false, false, 18, 5);
                schemaTable.Rows.Add(4, "DoubleColumn", typeof(Double), -1, false, false, 0, 0);
                schemaTable.Rows.Add(5, "FloatColumn", typeof(Single), -1, false, false, 0, 0);
                schemaTable.Rows.Add(6, "IntColumn", typeof(Int32), -1, false, false, 0, 0);
                schemaTable.Rows.Add(7, "LongColumn", typeof(Int64), -1, false, false, 0, 0);
                schemaTable.Rows.Add(8, "ShortColumn", typeof(Int16), -1, false, false, 0, 0);
                schemaTable.Rows.Add(9, "DateTimeColumn", typeof(DateTime), -1, false, false, 0, 0);

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
                3 => nameof(Decimal),
                4 => nameof(Double),
                5 => nameof(Single),
                6 => nameof(Int32),
                7 => nameof(Int64),
                8 => nameof(Int16),
                9 => nameof(DateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return ordinal switch
            {
                9 => _current.DateTimeColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override decimal GetDecimal(int ordinal)
        {
            return ordinal switch
            {
                3 => _current.DecimalColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override double GetDouble(int ordinal)
        {
            return ordinal switch
            {
                4 => _current.DoubleColumn,
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
                3 => typeof(Decimal),
                4 => typeof(Double),
                5 => typeof(Single),
                6 => typeof(Int32),
                7 => typeof(Int64),
                8 => typeof(Int16),
                9 => typeof(DateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override float GetFloat(int ordinal)
        {
            return ordinal switch
            {
                5 => _current.FloatColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override Guid GetGuid(int ordinal)
        {
            return ordinal switch
            {
                0 => _current.Qwerty,
                _ => throw new InvalidCastException(),
            };
        }

        public override short GetInt16(int ordinal)
        {
            return ordinal switch
            {
                8 => _current.ShortColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override int GetInt32(int ordinal)
        {
            return ordinal switch
            {
                6 => _current.IntColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override long GetInt64(int ordinal)
        {
            return ordinal switch
            {
                7 => _current.LongColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override string GetName(int ordinal)
        {
            return ordinal switch
            {
                0 => "Qwerty",
                1 => "BooleanColumn",
                2 => "StringColumn",
                3 => "DecimalColumn",
                4 => "DoubleColumn",
                5 => "FloatColumn",
                6 => "IntColumn",
                7 => "LongColumn",
                8 => "ShortColumn",
                9 => "DateTimeColumn",
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override int GetOrdinal(string name)
        {
            return name switch
            {
                "Qwerty" => 0,
                "BooleanColumn" => 1,
                "StringColumn" => 2,
                "DecimalColumn" => 3,
                "DoubleColumn" => 4,
                "FloatColumn" => 5,
                "IntColumn" => 6,
                "LongColumn" => 7,
                "ShortColumn" => 8,
                "DateTimeColumn" => 9,
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
        }

        public override string GetString(int ordinal)
        {
            return ordinal switch
            {
                2 => _current.StringColumn,
                _ => throw new InvalidCastException(),
            };
        }

        public override object GetValue(int ordinal)
        {
            return ordinal switch
            {
                0 => _current.Qwerty,
                1 => _current.BooleanColumn,
                2 => _current.StringColumn,
                3 => _current.DecimalColumn,
                4 => _current.DoubleColumn,
                5 => _current.FloatColumn,
                6 => _current.IntColumn,
                7 => _current.LongColumn,
                8 => _current.ShortColumn,
                9 => _current.DateTimeColumn,
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
                3 => false,
                4 => false,
                5 => false,
                6 => false,
                7 => false,
                8 => false,
                9 => false,
                _ => throw new ArgumentOutOfRangeException(nameof(ordinal))
            };
        }

        public override int FieldCount => 10;

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
}