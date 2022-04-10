using System.Collections.Generic;

namespace LinkShorter.WebApi.Models
{
    public record CustomTagsSettings
    {
        public List<CustomTagSchema> Tags { get; init; }
    }
}