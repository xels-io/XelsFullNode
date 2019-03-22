using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Xels.Bitcoin.Features.Api
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public MessageController(IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            this._hubContext = hubContext;
        }

        [HttpPost]
        public string Post([FromBody]int blockHeight)
        {
            string retMessage = string.Empty;

            try
            {
                this._hubContext.Clients.All.BroadcastMessage("", blockHeight.ToString());
                retMessage = "Success";
            }
            catch (Exception e)
            {
                retMessage = e.ToString();
            }

            return retMessage;
        }
    }

    public class Message
    {
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}
