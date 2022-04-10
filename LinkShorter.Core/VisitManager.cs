using LinkShorter.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShorter.Core
{
    public class VisitManager
    {
        private readonly DataContext data;

        public VisitManager(DataContext data)
        {
            this.data = data;
        }

        public async Task RegisterVisit(Link link)
        {
            var visit = new Visit
            {
                Date = DateTime.UtcNow,
                Link = link
            };

            data.Visits.Add(visit);
            await data.SaveChangesAsync();
        }

        public async Task<List<Visit>> GetVisits(DateTime since, DateTime until)
        {
            return await data.Visits
                .Where(visit => visit.Date >= since && visit.Date <= until)
                .Include(visit => visit.Link)
                .ThenInclude(link => link.CustomTags)
                .ToListAsync();
        }
    }
}