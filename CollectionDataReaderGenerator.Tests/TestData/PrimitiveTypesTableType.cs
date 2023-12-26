using System;

namespace CollectionDataReaderGenerator.Tests.TestData
{
    [GenerateDataReader]
    public partial struct PrimitiveTypesTableType
    {
        public Guid Qwerty { get; set; }
        public bool BooleanColumn { get; set; }
        [ColumnInfo(Length = 20, Ordinal = 2)]
        public string StringColumn { get; set; }

        [ColumnInfo(SqlTypeName = "varchar(10)")]
        public string StringColumn2 { get; set; }
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
