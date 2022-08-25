namespace LinkShorter.Core.Models
{
    public class Visit
    {
        public Guid Id { get; set; }
        public DateTime Date { get; init; }
        public Link Link { get; init; }
    }
}