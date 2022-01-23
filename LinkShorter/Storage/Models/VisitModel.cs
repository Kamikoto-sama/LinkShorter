using System;

namespace LinkShorter.Storage.Models
{
    public class VisitModel
    {
        public Guid Id { get; init; }
        public DateTime Date { get; init; }
        public LinkModel Link { get; init; }
    }
}