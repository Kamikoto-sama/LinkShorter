using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using LinkShorter.Export;
using LinkShorter.WebApi.Models;
using LinkShorter.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace LinkShorter.WebApi.Controllers
{
    [ApiController]
    [Route("/api/export")]
    public class ExportController : Controller
    {
        private readonly ExportManager exportManager;
        private readonly AccessKeyProvider accessKeyProvider;

        public ExportController(ExportManager exportManager, AccessKeyProvider accessKeyProvider)
        {
            this.exportManager = exportManager;
            this.accessKeyProvider = accessKeyProvider;
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateExport(string accessKey, DateTime since, DateTime until)
        {
            if (!accessKeyProvider.ValidateKey(accessKey))
                return Unauthorized();

            if (since == default)
                return BadRequest($"Invalid since date: {since}");
            if (until == default)
                return BadRequest($"Invalid until date: {until}");

            var result = await exportManager.CreateExportFileAsync(since, until);
            if (result.IsFailed)
                return BadRequest(result.Reasons.First().Message);

            var exportFile = result.Value;
            return File(exportFile.Content, MediaTypeNames.Text.Plain, exportFile.Name);
        }
    }
}