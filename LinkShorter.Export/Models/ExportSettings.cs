﻿namespace LinkShorter.Export.Models
{
    public record ExportSettings
    {
        public string FileType { get; init; }
        public string[] ExportFields { get; init; }
    }
}