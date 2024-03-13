using System;

namespace CollectionDataReaderGenerator.Tests.TestData
{
    [GenerateDataReader(SchemaName = "Test", TypeName = "Test_Type")]
    public partial struct PrimitiveTypesTableType
    {
        public Guid Qwerty { get; set; }
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
