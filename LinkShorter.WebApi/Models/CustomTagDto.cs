﻿using System.Runtime.Serialization;

namespace LinkShorter.WebApi.Models
{
    [DataContract]
    public record CustomTagDto
    {
        [DataMember(IsRequired = true)] public int Index { get; init; }
        [DataMember(IsRequired = true)] public string Value { get; init; }
    }
}