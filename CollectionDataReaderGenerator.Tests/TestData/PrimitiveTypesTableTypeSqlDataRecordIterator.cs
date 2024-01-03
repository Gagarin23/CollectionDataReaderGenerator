﻿// <auto-generated/>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient.Server;

namespace CollectionDataReaderGenerator.Tests.TestData
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
                new SqlMetaData("Qwerty", SqlDbType.UniqueIdentifier),
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
                record.SetGuid(0, current.Qwerty);
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