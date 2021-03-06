﻿using Microsoft.AspNetCore.Mvc;
using Xels.Bitcoin.AsyncWork;

namespace Xels.Bitcoin.Controllers
{
    /// <summary>
    /// Controller providing HTML Dashboard
    /// </summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IFullNode fullNode;
        private readonly IAsyncProvider asyncProvider;

        public DashboardController(IFullNode fullNode, IAsyncProvider asyncProvider)
        {
            this.fullNode = fullNode;
            this.asyncProvider = asyncProvider;
        }

        /// <summary>
        /// Gets a web page containing the last log output for this node.
        /// </summary>
        /// <returns>text/html content</returns>
        [HttpGet]
        [Route("Stats")]
        public IActionResult Stats()
        {
            string content = (this.fullNode as FullNode).LastLogOutput;
            return this.Content(content);
        }

        /// <summary>
        /// Returns a web page with Async Loops statistics
        /// </summary>
        /// <returns>text/html content</returns>
        [HttpGet]
        [Route("AsyncLoopsStats")]
        public IActionResult AsyncLoopsStats()
        {
            return this.Content(this.asyncProvider.GetStatistics(false));
        }
    }
}