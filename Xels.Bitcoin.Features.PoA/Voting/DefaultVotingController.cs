﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.Utilities;
using Xels.Bitcoin.Utilities.JsonErrors;
using Xels.Bitcoin.Utilities.ModelStateErrors;

namespace Xels.Bitcoin.Features.PoA.Voting
{
    [Route("api/[controller]")]
    public class DefaultVotingController : Controller
    {
        protected readonly IFederationManager fedManager;

        protected readonly VotingManager votingManager;

        protected readonly Network network;

        private readonly IPollResultExecutor pollExecutor;

        private readonly IWhitelistedHashesRepository whitelistedHashesRepository;

        protected readonly ILogger logger;

        public DefaultVotingController(IFederationManager fedManager, ILoggerFactory loggerFactory, VotingManager votingManager,
            IWhitelistedHashesRepository whitelistedHashesRepository, Network network, IPollResultExecutor pollExecutor)
        {
            this.fedManager = fedManager;
            this.votingManager = votingManager;
            this.whitelistedHashesRepository = whitelistedHashesRepository;
            this.network = network;
            this.pollExecutor = pollExecutor;

            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        [Route("fedmembers")]
        [HttpGet]
        public IActionResult GetFederationMembers()
        {
            try
            {
                List<string> hexList = this.fedManager.GetFederationMembers().Select(x => x.ToString()).ToList();

                return this.Json(hexList);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        [Route("pendingpolls")]
        [HttpGet]
        public IActionResult GetPendingPolls()
        {
            try
            {
                List<Poll> polls = this.votingManager.GetPendingPolls();

                IEnumerable<PollViewModel> models = polls.Select(x => new PollViewModel(x, this.pollExecutor));

                return this.Json(models);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        [Route("finishedpolls")]
        [HttpGet]
        public IActionResult GetFinishedPolls()
        {
            try
            {
                List<Poll> polls = this.votingManager.GetFinishedPolls();

                IEnumerable<PollViewModel> models = polls.Select(x => new PollViewModel(x, this.pollExecutor));

                return this.Json(models);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        [Route("whitelistedhashes")]
        [HttpGet]
        public IActionResult GetWhitelistedHashes()
        {
            try
            {
                string hashes = string.Join(Environment.NewLine, this.whitelistedHashesRepository.GetHashes().Select(x => x.ToString()).ToList());

                return this.Ok(hashes);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        [Route("schedulevote-whitelisthash")]
        [HttpPost]
        public IActionResult VoteWhitelistHash([FromBody]HashModel request)
        {
            return this.VoteWhitelistRemoveHashMember(request, true);
        }

        [Route("schedulevote-removehash")]
        [HttpPost]
        public IActionResult VoteRemoveHash([FromBody]HashModel request)
        {
            return this.VoteWhitelistRemoveHashMember(request, false);
        }

        private IActionResult VoteWhitelistRemoveHashMember(HashModel request, bool whitelist)
        {
            Guard.NotNull(request, nameof(request));

            if (!this.ModelState.IsValid)
                return ModelStateErrors.BuildErrorResponse(this.ModelState);

            if (!this.fedManager.IsFederationMember)
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Only federation members can vote", string.Empty);

            try
            {
                var hash = new uint256(request.Hash);

                this.votingManager.ScheduleVote(new VotingData()
                {
                    Key = whitelist ? VoteKey.WhitelistHash : VoteKey.RemoveHash,
                    Data = hash.ToBytes()
                });

                return this.Ok();
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "There was a problem executing a command.", e.ToString());
            }
        }

        [Route("scheduledvotes")]
        [HttpGet]
        public IActionResult GetScheduledVotes()
        {
            try
            {
                List<string> votes = this.votingManager.GetScheduledVotes().Select(x => x.Key.ToString()).ToList();

                return this.Json(votes);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
    }
}
