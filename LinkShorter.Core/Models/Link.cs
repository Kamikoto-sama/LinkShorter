namespace LinkShorter.Core.Models
{
    public class Link
    {
        public Guid Id { get; set; }
        public string Name { get; init; }
        public string OriginalUrl { get; init; }
        public List<CustomTag> CustomTags { get; init; }
    }
}