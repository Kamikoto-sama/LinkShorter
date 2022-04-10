using System.Text;
using LinkShorter.Export.Models;

namespace LinkShorter.Export
{
    public static class ExportFileBuilder
    {
        private const string ColumnSeparator = ";";

        public static async Task CreateCsvFileAsync(string filePath, int lineLength, IEnumerable<ExportLine> exportLines)
        {
            await using var fileStream = File.OpenWrite(filePath);

            var content = BuildContent(lineLength, exportLines);

            foreach (var contentByte in content)
                fileStream.WriteByte(contentByte);
            await fileStream.FlushAsync();
        }

        private static IEnumerable<byte> BuildContent(int lineLength, IEnumerable<ExportLine> exportLines)
        {
            return exportLines.SelectMany(exportLine =>
            {
                var lineValues = new string[lineLength];

                foreach (var (index, value) in exportLine.Values)
                    lineValues[index] = value;

                var line = string.Join(ColumnSeparator, lineValues);
                return Encode(line).Concat(Encode(Environment.NewLine));
            });
        }

        private static byte[] Encode(string value) => Encoding.UTF8.GetBytes(value);
    }
}