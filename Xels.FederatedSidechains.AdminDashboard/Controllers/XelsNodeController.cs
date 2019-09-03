using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xels.FederatedSidechains.AdminDashboard.Filters;
using Xels.FederatedSidechains.AdminDashboard.Services;
using Xels.FederatedSidechains.AdminDashboard.Settings;

namespace Xels.FederatedSidechains.AdminDashboard.Controllers
{
    [Route("xels-node")]
    public class XelsNodeController : Controller
    {
        private readonly DefaultEndpointsSettings defaultEndpointsSettings;

        public XelsNodeController(IOptions<DefaultEndpointsSettings> defaultEndpointsSettings)
        {
            this.defaultEndpointsSettings = defaultEndpointsSettings.Value;
        }

        [Ajax]
        [HttpPost]
        [Route("resync")]
        public async Task<IActionResult> ResyncAsync(string value)
        {
            bool isHeight = int.TryParse(value, out _);
            if (isHeight)
            {
                ApiResponse getblockhashRequest = await ApiRequester.GetRequestAsync(this.defaultEndpointsSettings.XelsNode, $"/api/Consensus/getblockhash?height={value}");
                ApiResponse syncRequest = await ApiRequester.PostRequestAsync(this.defaultEndpointsSettings.XelsNode, "/api/Wallet/sync", new { hash = ((string)getblockhashRequest.Content) });
                return syncRequest.IsSuccess ? (IActionResult)Ok() : BadRequest();
            }
            else
            {
                ApiResponse syncRequest = await ApiRequester.PostRequestAsync(this.defaultEndpointsSettings.XelsNode, "/api/Wallet/sync", new { hash = value });
                return syncRequest.IsSuccess ? (IActionResult)Ok() : BadRequest();
            }
        }

        [Ajax]
        [Route("resync-crosschain-transactions")]
        public async Task<IActionResult> ResyncCrosschainTransactionsAsync()
        {
            //TODO: implement this method
            ApiResponse stopNodeRequest = await ApiRequester.GetRequestAsync(this.defaultEndpointsSettings.XelsNode, "/api/Node/status");
            return stopNodeRequest.IsSuccess ? (IActionResult)Ok() : BadRequest();
        }

        [Ajax]
        [Route("stop")]
        public async Task<IActionResult> StopNodeAsync()
        {
            ApiResponse stopNodeRequest = await ApiRequester.PostRequestAsync(this.defaultEndpointsSettings.XelsNode, "/api/Node/stop", true);
            return stopNodeRequest.IsSuccess ? (IActionResult)Ok() : BadRequest();
        }

        [Ajax]
        [Route("change-log-level/{level}")]
        public async Task<IActionResult> ChangeLogLevelAsync(string rule, string level)
        {
            ApiResponse changeLogLevelRequest = await ApiRequester.PostRequestAsync(this.defaultEndpointsSettings.XelsNode, "/api/Node/loglevels", new { logRules = new[] { new { ruleName = rule, logLevel = level } } });
            return changeLogLevelRequest.IsSuccess ? (IActionResult)Ok() : BadRequest();
        }
    }
}
