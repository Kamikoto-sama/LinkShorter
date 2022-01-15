using System.Collections.Generic;

namespace LinkShorter.Models
{
    public record CustomTagsSettings
    {
        public List<CustomTagSchema> Tags { get; init; }
    }
}