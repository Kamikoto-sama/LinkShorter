using System;
using System.Collections.Generic;

namespace LinkShorter.Storage.Models
{
    public class LinkModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string OriginalUrl { get; init; }
        public List<CustomTagModel> CustomTags { get; init; }
    }
}