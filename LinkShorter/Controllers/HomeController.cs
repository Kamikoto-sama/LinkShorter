using System.Threading.Tasks;
using LinkShorter.Helpers;
using LinkShorter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LinkShorter.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly LinkManager linkManager;
        private readonly IOptionsSnapshot<CustomTagsSettings> options;

        public HomeController(LinkManager linkManager, IOptionsSnapshot<CustomTagsSettings> options)
        {
            this.linkManager = linkManager;
            this.options = options;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(options.Value.Tags);
        }

        [HttpGet("{linkName}")]
        public async Task<IActionResult> ResolveLink(string linkName)
        {
            try
            {
                var originalUrl = await linkManager.ResolveLink(linkName);
                return originalUrl != null ? Redirect(originalUrl) : NotFound();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}