using System;
using System.Linq;
using System.Threading.Tasks;
using LinkShorter.Helpers;
using LinkShorter.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkShorter.Controllers
{
    [ApiController]
    [Route("/api/export")]
    public class ExportController : Controller
    {
        private readonly ExportManager exportManager;

        public ExportController(ExportManager exportManager)
        {
            this.exportManager = exportManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateExport([FromBody] CreateExportDto model)
        {
            if (model.Since == default)
                return BadRequest($"Invalid since date: {model.Since}");
            if (model.Until == default)
                return BadRequest($"Invalid until date: {model.Until}");

            model.Until = model.Until.Add(TimeSpan.Parse("23:59:59"));
            var exportFilePath = await exportManager.CreateExportAsync(model.Since, model.Until);
            if (exportFilePath.IsFailed)
                return BadRequest(exportFilePath.Reasons.First().Message);
            return Ok(exportFilePath.Value);
        }
    }
}