using FluentResults;
using LinkShorter.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShorter.Core
{
    public class LinkManager
    {
        private readonly DataContext data;
        private readonly VisitManager visitManager;

        public LinkManager(DataContext data, VisitManager visitManager)
        {
            this.data = data;
            this.visitManager = visitManager;
        }

        public async Task<bool> CheckLinkNameExists(string linkName)
        {
            if (string.IsNullOrEmpty(linkName))
                return false;
            return await data.Links.AnyAsync(link => link.Name == linkName);
        }

        public async Task<string?> CreateLink(string originalUrl, string? linkName, List<CustomTag> customTags)
        {
            linkName ??= (await data.Links.CountAsync()).ToString()!;

            var linkModel = new Link
            {
                Name = linkName,
                OriginalUrl = originalUrl,
                CustomTags = customTags
            };

            data.Links.Add(linkModel);
            var result = await Result.Try(() => data.SaveChangesAsync());
            return result.IsSuccess ? linkName : null;
        }

        public async Task<Link?> GetLink(string linkName) => await data.Links.FirstOrDefaultAsync(x => x.Name == linkName);
    }
}