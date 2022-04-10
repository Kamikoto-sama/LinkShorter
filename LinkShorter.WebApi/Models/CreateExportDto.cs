using System;
using System.Runtime.Serialization;

namespace LinkShorter.WebApi.Models
{
    [DataContract]
    public record CreateExportDto
    {
        [DataMember(IsRequired = true)] public string AccessKey { get; init; }
        [DataMember(IsRequired = true)] public DateTime Since { get; init; }
        [DataMember(IsRequired = true)] public DateTime Until { get; set; }
    }
}