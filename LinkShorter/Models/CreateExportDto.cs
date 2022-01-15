using System;
using System.Runtime.Serialization;

namespace LinkShorter.Models
{
    [DataContract]
    public record CreateExportDto
    {
        [DataMember(IsRequired = true)] public DateTime Since { get; init; }
        [DataMember(IsRequired = true)] public DateTime Until { get; set; }
    }
}