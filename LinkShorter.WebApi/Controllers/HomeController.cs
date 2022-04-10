using System.Threading.Tasks;
using LinkShorter.Core;
using LinkShorter.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LinkShorter.WebApi.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly LinkManager linkManager;
        private readonly VisitManager visitManager;
        private readonly CustomTagsSettings options;

        public HomeController(LinkManager linkManager, VisitManager visitManager, IOptions<CustomTagsSettings> options)
        {
            this.linkManager = linkManager;
            this.visitManager = visitManager;
            this.options = options.Value;
        }

        [HttpGet]
        public IActionResult Index() => View(options.Tags);

        [HttpGet("{linkName}")]
        public async Task<IActionResult> ResolveLink(string linkName)
        {
            try
            {
                var link = await linkManager.GetLink(linkName);
                if (link == null)
                    return NotFound();

                await visitManager.RegisterVisit(link);

                var originalUrl = link.OriginalUrl;
                return Redirect(originalUrl);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}