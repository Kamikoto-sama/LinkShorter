using FluentResults;
using LinkShorter.Core;
using LinkShorter.Core.Models;
using LinkShorter.Export.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkShorter.Export;

public class ExportManager
{
    private readonly VisitManager visitManager;
    private readonly IOptionsSnapshot<ExportSettings> exportSettings;
    private readonly ILogger<ExportManager> logger;

    public ExportManager(VisitManager visitManager, IOptionsSnapshot<ExportSettings> exportSettings, ILogger<ExportManager> logger)
    {
        this.visitManager = visitManager;
        this.exportSettings = exportSettings;
        this.logger = logger;
    }

    public async Task<Result<ExportFile>> CreateExportFileAsync(DateTime since, DateTime until)
    {
        var linkStats = await Result.Try(() => GetLinkStats(since, until));
        if (linkStats.IsFailed)
        {
            logger.LogError(linkStats.Errors.First().Message);
            return Result.Fail("DataBase error");
        }

        if (linkStats.Value.Length <= 0)
            return Result.Fail("No visits to export!");

        var fileContent = await CreateExportFileContentAsync(linkStats.Value);

        var fileName = GetFileName();
        var export = new ExportFile(fileContent, exportSettings.Value.FileType, fileName);
        return Result.Ok(export);
    }

    private string GetFileName() => "DanLS_ClickReport.csv";

    private async Task<byte[]> CreateExportFileContentAsync(LinkStat[] linkStats)
    {
        var exportLines = linkStats.Select(linkStat =>
        {
            var link = linkStat.Link;
            var exportValues = new[]
                {
                    linkStat.VisitDate.ToString("dd.MM.yyyy"),
                    link.Created.ToString("dd.MM.yyyy"),
                    linkStat.VisitsCount.ToString(),
                    link.Name,
                    link.OriginalUrl
                }
                .Concat(link.CustomTags.OrderBy(x => x.Index).Select(tag => tag.Value))
                .Select((value, index) => new ExportLineValue(index, value));

            return new ExportLine(exportValues);
        });

        var headers = exportSettings.Value.ExportFields;
        var headersLine = new ExportLine(headers.Select((x, i) => new ExportLineValue(i, x)));

        return await CsvBuilder.CreateCsvContentAsync(headers.Length, exportLines.Prepend(headersLine).ToArray());
    }

    private async Task<LinkStat[]> GetLinkStats(DateTime since, DateTime until)
    {
        since = new DateTime(since.Year, since.Month, since.Day, 0, 0, 0, DateTimeKind.Utc);
        until = new DateTime(until.Year, until.Month, until.Day, 23, 59, 59, DateTimeKind.Utc);
        var visits = await visitManager.GetVisits(since, until);

        return visits
            .GroupBy(visit => visit.Date.Date)
            .OrderBy(x => x.Key)
            .SelectMany(dateGroup => dateGroup
                .GroupBy(visit => visit.Link)
                .Select(linkGroup => new LinkStat(linkGroup.Key, dateGroup.Key, linkGroup.Count()))
                .OrderBy(x => x.Link.Created)
            )
            .ToArray();
    }
}