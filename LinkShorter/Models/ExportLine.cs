using System.Collections.Generic;

namespace LinkShorter.Models
{
    public record ExportLine(IEnumerable<ExportLineValue> Values);
}