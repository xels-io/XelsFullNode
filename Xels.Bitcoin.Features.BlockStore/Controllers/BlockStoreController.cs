using System;
using System.Collections.Generic;
using System.Net;
using DBreeze.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.BitcoinCore;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Controllers.Models;
using Xels.Bitcoin.Features.BlockStore.AddressIndexing;
using Xels.Bitcoin.Features.BlockStore.Models;
using Xels.Bitcoin.Interfaces;
using Xels.Bitcoin.Utilities;
using Xels.Bitcoin.Utilities.JsonErrors;
using Xels.Bitcoin.Utilities.ModelStateErrors;

namespace Xels.Bitcoin.Features.BlockStore.Controllers
{
    public static class BlockStoreRouteEndPoint
    {
        public const string GetAddressesBalances = "getaddressesbalances";
        public const string GetVerboseAddressesBalances = "getverboseaddressesbalances";
        public const string GetAddressIndexerTip = "addressindexertip";
        public const string GetBlock = "block";
        public const string GetBlockCount = "GetBlockCount";
    }

    /// <summary>Controller providing operations on a blockstore.</summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]
    public class BlockStoreController : Controller
    {
        private readonly IAddressIndexer addressIndexer;

        /// <summary>Provides access to the block store on disk.</summary>
        private readonly IBlockStore blockStore;

        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>An interface that provides information about the chain and validation.</summary>
        private readonly IChainState chainState;

        /// <summary>The chain.</summary>
        private readonly ChainIndexer chainIndexer;

        /// <summary>Current network for the active controller instance.</summary>
        private readonly Network network;

        public BlockStoreController(
            Network network,
            ILoggerFactory loggerFactory,
            IBlockStore blockStore,
            IChainState chainState,
            ChainIndexer chainIndexer,
            IAddressIndexer addressIndexer)
        {
            Guard.NotNull(network, nameof(network));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(chainState, nameof(chainState));
            Guard.NotNull(addressIndexer, nameof(addressIndexer));

            this.addressIndexer = addressIndexer;
            this.network = network;
            this.blockStore = blockStore;
            this.chainState = chainState;
            this.chainIndexer = chainIndexer;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Retrieves the <see cref="addressIndexer"/>'s tip.
        /// </summary>
        /// <returns>An instance of <see cref="AddressIndexerTipModel"/> containing the tip's hash and height.</returns>
        [Route(BlockStoreRouteEndPoint.GetAddressIndexerTip)]
        [HttpGet]
        public IActionResult GetAddressIndexerTip()
        {
            try
            {
                ChainedHeader addressIndexerTip = this.addressIndexer.IndexerTip;
                return this.Json(new AddressIndexerTipModel() { TipHash = addressIndexerTip?.HashBlock, TipHeight = addressIndexerTip?.Height });
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        //[Route("getaddressoutpoint")]
        //[HttpGet]
        //public IActionResult GetAddressOutpoint(string address)
        //{
        //    AddressIndexerData indexData = this.addressIndexer.GetAddressIndexRepo().GetOrCreateAddress(address);
        //    indexData.
        //    //this.addressIndexer.get
        //}


        /// <summary>
        /// Retrieves the block which matches the supplied block hash.
        /// </summary>
        /// <param name="query">An object containing the necessary parameters to search for a block.</param>
        /// <returns><see cref="BlockModel"/> if block is found, <see cref="NotFoundObjectResult"/> if not found. Returns <see cref="IActionResult"/> with error information if exception thrown.</returns>
        [Route(BlockStoreRouteEndPoint.GetBlock)]
        [HttpGet]
        public IActionResult GetBlock([FromQuery] SearchByHashRequest query)
        {
            if (!this.ModelState.IsValid)
                return ModelStateErrors.BuildErrorResponse(this.ModelState);

            try
            {
                uint256 blockId = uint256.Parse(query.Hash);

                ChainedHeader chainedHeader = this.chainIndexer.GetHeader(blockId);

                if (chainedHeader == null)
                    return this.Ok("Block not found");

                Block block = chainedHeader.Block ?? this.blockStore.GetBlock(blockId);

                // In rare occasions a block that is found in the
                // indexer may not have been pushed to the store yet. 
                if (block == null)
                    return this.Ok("Block not found");

                if (!query.OutputJson)
                {
                    return this.Json(block);
                }

                BlockModel blockModel = query.ShowTransactionDetails
                    ? new BlockTransactionDetailsModel(block, chainedHeader, this.chainIndexer.Tip, this.network)
                    : new BlockModel(block, chainedHeader, this.chainIndexer.Tip, this.network);


                blockModel.BlockReward = GetRewardFromHeight(blockModel.Height);

                if (this.network.Consensus.IsProofOfStake)
                {
                    var posBlock = block as PosBlock;

                    blockModel.PosBlockSignature = posBlock.BlockSignature.ToHex(this.network);
                    blockModel.PosBlockTrust = new Target(chainedHeader.GetBlockProof()).ToUInt256().ToString();
                    blockModel.PosChainTrust = chainedHeader.ChainWork.ToString(); // this should be similar to ChainWork
                }

                return this.Json(blockModel);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
        

        [Route("getallblocksfromheight")]
        [HttpGet]
        public IActionResult GetAllBlocksFromHeight(int height, bool showTransactionDetails = false)
        {
            try
            {
                List<BlockModel> lstBlockModel = new List<BlockModel>();
                List<ChainedHeader> lstChainedHeader = this.chainIndexer.GetRestBlocks(height);

                foreach (ChainedHeader chainedHeader in lstChainedHeader)
                {
                    if (chainedHeader == null)
                        continue;

                    Block block = chainedHeader.Block ?? this.blockStore.GetBlock(chainedHeader.HashBlock);

                    // In rare occasions a block that is found in the
                    // indexer may not have been pushed to the store yet. 
                    if (block == null)
                        continue;

                    BlockModel blockModel = showTransactionDetails
                        ? new BlockTransactionDetailsModel(block, chainedHeader, this.chainIndexer.Tip, this.network)
                        : new BlockModel(block, chainedHeader, this.chainIndexer.Tip, this.network);

                    if (this.network.Consensus.IsProofOfStake)
                    {
                        var posBlock = block as PosBlock;

                        blockModel.PosBlockSignature = posBlock.BlockSignature.ToHex(this.network);
                        blockModel.PosBlockTrust = new Target(chainedHeader.GetBlockProof()).ToUInt256().ToString();
                        blockModel.PosChainTrust = chainedHeader.ChainWork.ToString(); // this should be similar to ChainWork
                    }
                    blockModel.BlockReward = GetRewardFromHeight(height);
                    lstBlockModel.Add(blockModel);
                }
                return this.Json(lstBlockModel);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        /// <summary>
        /// Neo: We've to find some way to invoke this method from PosCoinViewRule
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        private Money GetRewardFromHeight(int height)
        {
            if (height <= this.network.Consensus.PremineHeight)
            {
                return this.network.Consensus.PremineReward;
            }
            else if (height <= this.network.Consensus.FirstMiningPeriodHeight)
            {
                return this.network.Consensus.ProofOfStakeReward;
            }
            else if (height <= this.network.Consensus.SecondMiningPeriodHeight)
            {
                return this.network.Consensus.ProofOfStakeReward - (Money.Satoshis(3256) * (height - this.network.Consensus.FirstMiningPeriodHeight));
            }
            else if (height <= this.network.Consensus.ThirdMiningPeriodHeight)
            {
                return this.network.Consensus.ProofOfStakeReward / 2;
            }
            else if (height <= this.network.Consensus.ForthMiningPeriodHeight)
            {
                return (this.network.Consensus.ProofOfStakeReward / 2) - (Money.Satoshis(1628) * (height - this.network.Consensus.ThirdMiningPeriodHeight));
            }
            else if (height <= this.network.Consensus.FifthMiningPeriodHeight)
            {
                return this.network.Consensus.ProofOfStakeReward / 4;
            }
            else
            {
                int multiplier = (int)(height - this.network.Consensus.FifthMiningPeriodHeight) / (int)210240;
                double returnAmount = 1449770000;

                if (multiplier == 0)
                {
                    return Money.Satoshis(1449770000);
                }
                else
                {
                    for (int i = 0; i < multiplier; i++)
                    {
                        returnAmount *= 1.02;
                    }
                }
                return Money.Satoshis((decimal)returnAmount);
            }
        }

        /// <summary>
        /// Gets the current consensus tip height.
        /// </summary>
        /// <remarks>This is an API implementation of an RPC call.</remarks>
        /// <returns>The current tip height. Returns <c>null</c> if fails. Returns <see cref="IActionResult"/> with error information if exception thrown.</returns>
        [Route(BlockStoreRouteEndPoint.GetBlockCount)]
        [HttpGet]
        public IActionResult GetBlockCount()
        {
            try
            {
                return this.Json(this.chainState.ConsensusTip.Height);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        /// <summary>Provides balance of the given addresses confirmed with at least <paramref name="minConfirmations"/> confirmations.</summary>
        /// <param name="addresses">A comma delimited set of addresses that will be queried.</param>
        /// <param name="minConfirmations">Only blocks below consensus tip less this parameter will be considered.</param>
        /// <returns>A result object containing the balance for each requested address and if so, a meesage stating why the indexer is not queryable.</returns>
        [Route(BlockStoreRouteEndPoint.GetAddressesBalances)]
        [HttpGet]
        public IActionResult GetAddressesBalances(string addresses, int minConfirmations)
        {
            try
            {
                string[] addressesArray = addresses.Split(',');

                this.logger.LogDebug("Asking data for {0} addresses.", addressesArray.Length);

                AddressBalancesResult result = this.addressIndexer.GetAddressBalances(addressesArray, minConfirmations);

                this.logger.LogDebug("Sending data for {0} addresses.", result.Balances.Count);

                return this.Json(result);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }


        /// <summary>Provides verbose balance data of the given addresses.</summary>
        /// <param name="addresses">A comma delimited set of addresses that will be queried.</param>
        /// <returns>A result object containing the balance for each requested address and if so, a meesage stating why the indexer is not queryable.</returns>
        [Route(BlockStoreRouteEndPoint.GetVerboseAddressesBalances)]
        [HttpGet]
        public IActionResult GetVerboseAddressesBalancesData(string addresses)
        {
            try
            {
                string[] addressesArray = addresses?.Split(',') ?? new string[] { };

                this.logger.LogDebug("Asking data for {0} addresses.", addressesArray.Length);

                VerboseAddressBalancesResult result = this.addressIndexer.GetAddressIndexerState(addressesArray);

                return this.Json(result);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
    }
}
