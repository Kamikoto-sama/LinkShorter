using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LinkShorter.WebApi.Models
{
    [DataContract]
    public record LinkCreateDto
    {
        [DataMember(IsRequired = true)] public string AccessKey { get; init; }
        [DataMember(IsRequired = true)] public string OriginalUrl { get; init; }
        [DataMember] public string ShortLinkName { get; init; }
        [DataMember] public List<CustomTagDto> CustomTags { get; init; }
    }
}