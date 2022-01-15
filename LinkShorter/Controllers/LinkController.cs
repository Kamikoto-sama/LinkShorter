﻿using System;
using System.Threading.Tasks;
using LinkShorter.Helpers;
using LinkShorter.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkShorter.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class LinkController : Controller
    {
        private readonly AccessKeyProvider accessKeyProvider;
        private readonly LinkManager linkManager;

        public LinkController(LinkManager linkManager, AccessKeyProvider accessKeyProvider)
        {
            this.linkManager = linkManager;
            this.accessKeyProvider = accessKeyProvider;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] LinkCreateDto model)
        {
            if (!accessKeyProvider.ValidateKey(model.AccessKey))
                return Unauthorized();
            if (string.IsNullOrEmpty(model.OriginalUrl) || !Uri.IsWellFormedUriString(model.OriginalUrl, UriKind.Absolute))
                return BadRequest($"Invalid original url: {model.OriginalUrl}");

            var linkName = model.ShortLinkName;
            if (string.IsNullOrEmpty(linkName))
                linkName = null;
            if (linkName != null && !Uri.IsWellFormedUriString(model.ShortLinkName, UriKind.Relative))
                return BadRequest($"Invalid short link name: {model.ShortLinkName}");

            var exists = await linkManager.CheckLinkNameExists(model.ShortLinkName);
            if (exists)
                return BadRequest($"Such link name already exists: {model.ShortLinkName}");

            linkName = await linkManager.CreateLink(model.OriginalUrl, linkName, model.CustomTags);
            return linkName == null ? BadRequest("Cannot create link. Try again later") : Ok(linkName);
        }
    }
}