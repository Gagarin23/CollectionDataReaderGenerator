using System.Data;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MassTransit;
using Microsoft.Data.SqlClient;

namespace CollectionDataReaderGenerator.Bench;

/*

| Method                    | Mean       | Error    | StdDev    | Gen0   | Gen1   | Allocated |
|-------------------------- |-----------:|---------:|----------:|-------:|-------:|----------:|
| EntityFramework7OrLower   |   858.8 us | 31.47 us |  44.11 us | 1.9531 | 1.9531 |  46.78 KB |
| EntityFramework8OrGreater | 7,818.9 us | 78.47 us | 117.45 us |      - |      - |  18.86 KB |
| CollectionDataReader      | 1,464.5 us | 15.82 us |  23.19 us |      - |      - |  20.93 KB |
| SqlDataRecordIterator     | 1,449.7 us | 10.19 us |  14.28 us |      - |      - |   9.07 KB |

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
            await EntityFramework7OrLower();
            await EntityFramework8OrGreater();
            await CollectionDataReader();
            await SqlDataRecordIterator();
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
    public async Task EntityFramework7OrLower()
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
    public async Task EntityFramework8OrGreater()
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
    public async Task CollectionDataReader()
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

    [Benchmark] public async Task SqlDataRecordIterator()
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