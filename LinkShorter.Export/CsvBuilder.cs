using LinkShorter.Export.Models;

namespace LinkShorter.Export
{
    public static class CsvBuilder
    {
        private const string ColumnSeparator = ";";

        public static async Task<byte[]> CreateCsvContentAsync(int headersCount, ExportLine[] exportLines)
        {
            var fileStream = new MemoryStream(headersCount * exportLines.Length);
            await using var fileWriter = new StreamWriter(fileStream);

            foreach (var line in exportLines)
            {
                var lineValues = new string[headersCount];
                foreach (var (index, value) in line.Values)
                    lineValues[index] = value;
                await fileWriter.WriteLineAsync(string.Join(ColumnSeparator, lineValues));
            }

            await fileWriter.FlushAsync();
            return fileStream.ToArray();
        }
    }
}