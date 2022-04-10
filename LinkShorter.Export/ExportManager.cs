using FluentResults;
using LinkShorter.Core;
using LinkShorter.Core.Models;
using LinkShorter.Export.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkShorter.Export
{
    public class ExportManager
    {
        private const string ExportDir = "~/storage/export";
        private readonly VisitManager visitManager;
        private readonly IOptionsSnapshot<ExportSettings> exportSettings;
        private readonly ILogger<ExportManager> logger;

        public ExportManager(VisitManager visitManager, IOptionsSnapshot<ExportSettings> exportSettings, ILogger<ExportManager> logger)
        {
            this.visitManager = visitManager;
            this.exportSettings = exportSettings;
            this.logger = logger;
        }

        public async Task<Result<string>> CreateExportAsync(DateTime since, DateTime until)
        {
            until = until.Add(TimeSpan.Parse("23:59:59"));

            var visits = await Result.Try(() => GetGroupedVisits(since, until));
            if (visits.IsFailed)
            {
                logger.LogError(visits.Errors.First().Message);
                return Result.Fail("DataBase error");
            }

            if (visits.Value.Count <= 0)
                return Result.Fail("No visits to export!");

            var path = GetExportFilePath();
            await CreateExportFileAsync(visits.Value, path);

            return Result.Ok(path);
        }

        private string GetExportFilePath()
        {
            Directory.CreateDirectory(ExportDir);

            var fileType = exportSettings.Value.FileType;
            var date = DateTime.UtcNow.ToString("dd-MM-yyyy");
            var hash = new Random().Next(int.MinValue, int.MaxValue).ToString("x8");
            var fileName = $"{date}-{hash}.{fileType}";

            return Path.Combine(ExportDir, fileName);
        }

        private async Task CreateExportFileAsync(List<IEnumerable<IGrouping<string, Visit>>> visitDateGroups, string filePath)
        {
            var exportLines = visitDateGroups.SelectMany(dateGroup => dateGroup
                .Select(idGroup =>
                {
                    var dayVisits = idGroup.ToList();
                    var visit = idGroup.First();
                    var exportValues = new[]
                        {
                            visit.Date.ToString("dd.MM.yyyy"),
                            dayVisits.Count.ToString(),
                            visit.Link.Name,
                            visit.Link.OriginalUrl
                        }
                        .Concat(visit.Link.CustomTags.Select(tag => tag.Value))
                        .Select((value, index) => new ExportLineValue(index, value));

                    return new ExportLine(exportValues);
                }));

            var headers = exportSettings.Value.ExportFields;
            var headersLine = new ExportLine(headers.Select((x, i) => new ExportLineValue(i, x)));
            await ExportFileBuilder.CreateCsvFileAsync(filePath, headers.Length, exportLines.Prepend(headersLine));
        }

        private async Task<List<IEnumerable<IGrouping<string, Visit>>>> GetGroupedVisits(DateTime since, DateTime until)
        {
            var visits = await visitManager.GetVisits(since, until);
            return visits
                .GroupBy(visit => visit.Date.Date)
                .OrderBy(x => x.Key)
                .Select(dateGroup => dateGroup.GroupBy(visit => visit.Link.Name))
                .ToList();
        }
    }
}