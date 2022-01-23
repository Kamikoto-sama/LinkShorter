using System;

namespace LinkShorter.Storage.Models
{
    public class CustomTagModel
    {
        public Guid Id { get; init; }
        public int Index { get; init; }
        public string Value { get; init; }
    }
}