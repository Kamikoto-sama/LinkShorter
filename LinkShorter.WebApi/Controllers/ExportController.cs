using System;
using System.Linq;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateExport([FromBody] CreateExportDto model)
        {
            if (!accessKeyProvider.ValidateKey(model.AccessKey))
                return Unauthorized();

            if (model.Since == default)
                return BadRequest($"Invalid since date: {model.Since}");
            if (model.Until == default)
                return BadRequest($"Invalid until date: {model.Until}");

            var exportFilePath = await exportManager.CreateExportAsync(model.Since, model.Until);
            if (exportFilePath.IsFailed)
                return BadRequest(exportFilePath.Reasons.First().Message);
            return Ok(exportFilePath.Value);
        }
    }
}