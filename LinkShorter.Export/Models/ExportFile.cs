namespace LinkShorter.Export.Models
{
    public record ExportFile(byte[] Content, string Type, string Name);
}