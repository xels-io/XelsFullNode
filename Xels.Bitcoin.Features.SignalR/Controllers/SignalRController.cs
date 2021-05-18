using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xels.Bitcoin.Utilities;
using Xels.Bitcoin.Utilities.JsonErrors;

namespace Xels.Bitcoin.Features.SignalR.Controllers
{
    /// <summary>
    /// Controller for connecting to SignalR.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SignalRController : ControllerBase
    {
        private readonly SignalRSettings signalRSettings;
        private readonly ILogger logger;

        public SignalRController(SignalRSettings signalRSettings, ILoggerFactory loggerFactory)
        {
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(signalRSettings, nameof(signalRSettings));

            this.signalRSettings = signalRSettings;
            this.logger = loggerFactory.CreateLogger<SignalRController>();
        }

        /// <summary>
        /// Returns SignalR Connection Info.
        /// </summary>
        /// <returns>Returns SignalR Connection Info as Json {SignalRUri,SignalPort}</returns>
        [Route("getConnectionInfo")]
        [HttpGet]
        public IActionResult GetConnectionInfo()
        {
            try
            {
                return this.Ok(new
                {
                    this.signalRSettings.SignalRUri,
                    this.signalRSettings.SignalPort
                });
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
    }
}