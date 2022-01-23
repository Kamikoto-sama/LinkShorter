using System;
using System.Threading.Tasks;
using LinkShorter.Storage;
using LinkShorter.Storage.Models;

namespace LinkShorter.Helpers
{
    public class VisitManager
    {
        private readonly StorageContext storage;

        public VisitManager(StorageContext storage)
        {
            this.storage = storage;
        }

        public async Task RegisterVisit(LinkModel link)
        {
            var visit = new VisitModel
            {
                Date = DateTime.UtcNow,
                Link = link
            };

            storage.Visits.Add(visit);
            await storage.SaveChangesAsync();
        }
    }
}