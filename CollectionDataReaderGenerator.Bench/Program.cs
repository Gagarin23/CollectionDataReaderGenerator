using BenchmarkDotNet.Running;
using CollectionDataReaderGenerator.Bench;
using DotNet.Testcontainers.Builders;
using Microsoft.Data.SqlClient;

var dbContainer = new ContainerBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPortBinding(60303, 1433)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_SA_PASSWORD", "123qwe!@#QWE")
            .WithAutoRemove(true)
            .WithWaitStrategy(Wait.ForUnixContainer())
            .Build();

await dbContainer.StartAsync();

// Wait for SQL Server to start
await Task.Delay(10000);

await using (var connection = new SqlConnection("Server=localhost,60303;Database=master;User Id=sa;Password=123qwe!@#QWE;Integrated Security=false;TrustServerCertificate=true;"))
{
    await using var createDbCmd = connection.CreateCommand();
    createDbCmd.CommandText = "CREATE DATABASE TestDB;";

    await connection.OpenAsync();
    try
    {
        await createDbCmd.ExecuteNonQueryAsync();
    }
    finally
    {
        await connection.CloseAsync();
    }
}

await using (var connection = new SqlConnection("Server=localhost,60303;Database=TestDB;User Id=sa;Password=123qwe!@#QWE;Integrated Security=false;TrustServerCertificate=true;"))
{
    await using var createTestTableCmd = connection.CreateCommand();
    createTestTableCmd.CommandText = @"
CREATE TABLE TestTable
(
    Id uniqueidentifier PRIMARY KEY 
);";

    await using var createTestTypeCmd = connection.CreateCommand();
    createTestTypeCmd.CommandText = PrimitiveTableType.CreateTableTypeSqlText;

    using var bulk = new SqlBulkCopy(connection);
    bulk.DestinationTableName = "TestTable";
    var reader = PrimitiveTableType.CreateDataReader(MssqlDriverLevelBenchmark.TestData);

    await using var cacheCommand = connection.CreateCommand();
    cacheCommand.CommandText = "SELECT * FROM TestTable;";

    await connection.OpenAsync();

    try
    {
        await createTestTableCmd.ExecuteNonQueryAsync();
        await createTestTypeCmd.ExecuteNonQueryAsync();
        await bulk.WriteToServerAsync(reader);
        await using var _ = await cacheCommand.ExecuteReaderAsync();
    }
    finally
    {
        await connection.CloseAsync();
    }
}

var bench = new MssqlDriverLevelBenchmark();

#if DEBUG
    await bench.TestRun();
#else
    bench.Run();
#endif

