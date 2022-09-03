using LinkShorter.Core.Models;

namespace LinkShorter.Export.Models;

public record LinkStat(Link Link, DateTime VisitDate, int VisitsCount);