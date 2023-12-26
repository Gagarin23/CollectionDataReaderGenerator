using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CollectionDataReaderGenerator.Tests.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Data.SqlClient.Server;
using Xunit;

namespace CollectionDataReaderGenerator.Tests
{
    public class CollectionDataReaderGeneratorTests
    {
        [Fact]
        public void GenerateReportMethod()
        {
            // Create an instance of the source generator.
            var generator = new CollectionDataReaderIncrementalGenerator();

            // Source generators should be tested using 'GeneratorDriver'.
            var driver = CSharpGeneratorDriver.Create(generator);

            string userFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            string netStandardPath = Path.Combine(userFolderPath, ".nuget\\packages\\netstandard.library\\2.0.3\\build\\netstandard2.0\\ref");

            var source = new TestAdditionalFile("TestData/PrimitiveTypesTableType.cs");

            // We need to create a compilation with the required source code.
            var compilation = CSharpCompilation.Create
            (
                Assembly.GetAssembly(typeof(CollectionDataReaderIncrementalGenerator)).FullName,
                new[] { CSharpSyntaxTree.ParseText(source.GetText()) },
                new[]
                {
                    MetadataReference.CreateFromFile(Path.Combine(netStandardPath, "netstandard.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(netStandardPath, "System.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(netStandardPath, "mscorlib.dll")),
                    MetadataReference.CreateFromFile(typeof(CollectionDataReaderIncrementalGenerator).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(SqlDataRecord).Assembly.Location),
                }
            );

            // Run generators and retrieve all results.
            var runResult = driver.RunGenerators(compilation)
                .GetRunResult();

            // All generated files can be found in 'RunResults.GeneratedTrees'.
            var generatedDataReaderFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("PrimitiveTypesTableTypeDataReader.g.cs"));
            var generatedPartialFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("PrimitiveTypesTableType.g.cs"));
            var generatedSqlRecordEnumeratorFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("PrimitiveTypesTableTypeSqlDataRecordIterator.g.cs"));

            // Complex generators should be tested using text comparison.
            Assert.Equal
            (
                Regex.Replace(File.ReadAllText("TestData/PrimitiveTypesTableTypeDataReaderExpect.cs"), @"[\r\n\s]", ""),
                Regex.Replace(generatedDataReaderFileSyntax.GetText().ToString(), @"[\r\n\s]", "")
            );
            Assert.Equal
            (
                Regex.Replace(File.ReadAllText("TestData/PrimitiveTypesTableTypeExpect.cs"), @"[\r\n\s]", ""),
                Regex.Replace(generatedPartialFileSyntax.GetText().ToString(), @"[\r\n\s]", "")
            );
            Assert.Equal
            (
                Regex.Replace(File.ReadAllText("TestData/PrimitiveTypesTableTypeSqlDataRecordIterator.cs"), @"[\r\n\s]", ""),
                Regex.Replace(generatedSqlRecordEnumeratorFileSyntax.GetText().ToString(), @"[\r\n\s]", "")
            );
        }
    }
}
