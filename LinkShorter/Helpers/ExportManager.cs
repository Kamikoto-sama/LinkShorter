using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using LinkShorter.Models;
using LinkShorter.Storage;
using LinkShorter.Storage.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkShorter.Helpers
{
    public class ExportManager
    {
        private const string ExportDir = "storage/export";

        private readonly StorageContext storage;
        private readonly IOptionsSnapshot<ExportSettings> exportSettings;
        private readonly ILogger<ExportManager> logger;
        private readonly IWebHostEnvironment env;

        public ExportManager(StorageContext storage, IOptionsSnapshot<ExportSettings> exportSettings, ILogger<ExportManager> logger, IWebHostEnvironment env)
        {
            this.storage = storage;
            this.exportSettings = exportSettings;
            this.logger = logger;
            this.env = env;
        }

        public async Task<Result<string>> CreateExportAsync(DateTime since, DateTime until)
        {
            var visits = await Result.Try(() => GetVisits(since, until));
            if (visits.IsFailed)
            {
                logger.LogError(visits.Errors.First().Message);
                return Result.Fail("DataBase error");
            }

            if (visits.Value.Count <= 0)
                return Result.Fail("No visitDateGroups to export");

            var (full, relative) = GetExportFilePath();
            await CreateExportFileAsync(visits.Value, full);

            return Result.Ok(relative);
        }

        private (string Full, string Relative) GetExportFilePath()
        {
            Directory.CreateDirectory(Path.Combine(env.WebRootPath, ExportDir));

            var fileType = exportSettings.Value.FileType;
            var date = DateTime.UtcNow.ToString("dd-MM-yyyy");
            var hash = new Random().Next(int.MinValue, int.MaxValue).ToString("x8");
            var fileName = $"{date}-{hash}.{fileType}";

            var relative = $"{ExportDir}/{fileName}";
            var full = Path.Combine(env.WebRootPath, ExportDir, fileName);

            return (full, relative);
        }

        private async Task CreateExportFileAsync(List<IEnumerable<IGrouping<Guid,VisitModel>>> visitDateGroups, string filePath)
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

        private async Task<List<IEnumerable<IGrouping<Guid,VisitModel>>>> GetVisits(DateTime since, DateTime until)
        {
            var visits = await storage.Visits
                .Where(visit => visit.Date >= since && visit.Date <= until)
                .Include(visit => visit.Link)
                .ThenInclude(link => link.CustomTags)
                .ToListAsync();
            return visits
                .GroupBy(visit => visit.Date.Date)
                .OrderBy(x => x.Key)
                .Select(dateGroup => dateGroup.GroupBy(visit => visit.Link.Id))
                .ToList();
        }
    }
}