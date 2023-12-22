using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace CollectionDataReaderGenerator.Tests.Utils
{
    public class TestAdditionalFile : AdditionalText
    {
        private readonly SourceText _text;

        public TestAdditionalFile(string path)
        {
            Path = path;
            _text = SourceText.From(File.ReadAllText(path));
        }

        public override SourceText GetText(CancellationToken cancellationToken = new()) => _text;

        public override string Path { get; }
    }
}
