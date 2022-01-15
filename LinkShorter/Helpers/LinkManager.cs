using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using LinkShorter.Models;
using LinkShorter.Storage;
using LinkShorter.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShorter.Helpers
{
    public class LinkManager
    {
        private readonly StorageContext storage;
        private readonly VisitManager visitManager;

        public LinkManager(StorageContext storage, VisitManager visitManager)
        {
            this.storage = storage;
            this.visitManager = visitManager;
        }

        public async Task<bool> CheckLinkNameExists(string linkName)
        {
            if (string.IsNullOrEmpty(linkName))
                return false;
            return await storage.Links.AnyAsync(link => link.Name == linkName);
        }

        public async Task<string> CreateLink(string originalUrl, string linkName, IEnumerable<CustomTagDto> customTags)
        {
            linkName ??= LinkNameGenerator.Generate(await storage.Links.CountAsync());
            var linkModel = new LinkModel
            {
                Name = linkName,
                OriginalUrl = originalUrl,
                CustomTags = customTags?.Select(tag => new CustomTagModel
                {
                    Index = tag.Index,
                    Value = tag.Value
                }).ToList()
            };

            storage.Links.Add(linkModel);
            var result = await Result.Try(() => storage.SaveChangesAsync());
            return result.IsSuccess ? linkName : null;
        }

        public async Task<string> ResolveLink(string linkName)
        {
            var linkModel = await storage.Links.FirstOrDefaultAsync(x => x.Name == linkName);
            if (linkModel == null)
                return null;

            visitManager.RegisterVisit(linkModel);

            return linkModel.OriginalUrl;
        }
    }
}