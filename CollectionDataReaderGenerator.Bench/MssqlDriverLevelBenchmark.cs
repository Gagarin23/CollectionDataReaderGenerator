using System.Data;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MassTransit;
using Microsoft.Data.SqlClient;

namespace CollectionDataReaderGenerator.Bench;

/*

| Method                                   | Mean       | Error    | StdDev   | Gen0   | Gen1   | Allocated |
|----------------------------------------- |-----------:|---------:|---------:|-------:|-------:|----------:|
| AdhocQuery                               |   787.1 us |  2.88 us |  3.94 us | 1.9531 | 1.9531 |  46.78 KB |
| ParameterizedJsonQuery                   | 7,183.8 us | 19.92 us | 28.57 us |      - |      - |  18.86 KB |
| ParameterizedByCollectionDataReaderQuery | 1,370.4 us |  5.14 us |  7.38 us |      - |      - |  20.94 KB |
| ParameterizedSqlDataRecordIteratorQuery  | 1,365.2 us |  7.19 us | 10.76 us |      - |      - |   9.07 KB |

 */

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class MssqlDriverLevelBenchmark
{
    public static readonly List<PrimitiveTableType> TestData = Enumerable.Range(0, 100)
        .Select(x => new PrimitiveTableType {Id = NewId.NextGuid()})
        .ToList();

    private SqlConnection _connection;

    public void Run()
    {
        BenchmarkRunner.Run<MssqlDriverLevelBenchmark>();
    }

    public async Task TestRun()
    {
        Setup();
        try
        {
            await AdhocQuery();
            await ParameterizedJsonQuery();
            await ParameterizedByCollectionDataReaderQuery();
            await ParameterizedSqlDataRecordIteratorQuery();
        }
        finally
        {
            Cleanup();
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        _connection = new SqlConnection("Server=localhost,60303;Database=TestDB;User ID=sa;Password=123qwe!@#QWE;Integrated Security=false;TrustServerCertificate=true;");
        _connection.Open();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _connection.Close();
        _connection.Dispose();
    }

    [Benchmark]
    public async Task AdhocQuery()
    {
        await using var command = _connection.CreateCommand();
        command.CommandText = @$"
SELECT 
    Id 
FROM TestTable
WHERE Id IN (
    {string.Join(", ", TestData.Select(x => $"'{x.Id}'"))}
)";

        await using var _ = await command.ExecuteReaderAsync();
    }

    [Benchmark]
    public async Task ParameterizedJsonQuery()
    {
        await using var command = _connection.CreateCommand();
        command.Parameters.Add(new SqlParameter()
        {
            ParameterName = "@Ids",
            Value = JsonSerializer.Serialize(TestData),
            SqlDbType = SqlDbType.NVarChar
        });
        command.CommandText = @"
SELECT 
    Id 
FROM TestTable
WHERE EXISTS (
    SELECT
        1
    FROM OPENJSON(@Ids)
    WITH (jsonId uniqueidentifier '$')
    WHERE jsonId = Id
)";

        await using var _ = await command.ExecuteReaderAsync();
    }

    [Benchmark]
    public async Task ParameterizedByCollectionDataReaderQuery()
    {
        await using var command = _connection.CreateCommand();
        command.Parameters.Add(new SqlParameter()
        {
            ParameterName = "@Ids",
            Value = PrimitiveTableType.CreateDataReader(TestData),
            SqlDbType = SqlDbType.Structured,
            TypeName = PrimitiveTableType.TableTypeName
        });
        command.CommandText = @"
SELECT 
    TestTable.Id 
FROM @Ids t
INNER JOIN TestTable ON TestTable.Id = t.Id";

        await using var _ = await command.ExecuteReaderAsync();
    }

    [Benchmark]
    public async Task ParameterizedSqlDataRecordIteratorQuery()
    {
        await using var command = _connection.CreateCommand();
        command.Parameters.Add(new SqlParameter()
        {
            ParameterName = "@Ids",
            Value = PrimitiveTableType.CreateSqlDataRecordIterator(TestData),
            SqlDbType = SqlDbType.Structured,
            TypeName = PrimitiveTableType.TableTypeName
        });
        command.CommandText = @"
SELECT 
    TestTable.Id 
FROM @Ids t
INNER JOIN TestTable ON TestTable.Id = t.Id";

        await using var _ = await command.ExecuteReaderAsync();
    }
}