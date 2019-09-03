﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Controllers;
using Xels.Bitcoin.Primitives;
using Xels.Features.FederatedPeg.Interfaces;
using Xels.Features.FederatedPeg.Models;

namespace Xels.Features.FederatedPeg.SourceChain
{
    public interface IMaturedBlocksProvider
    {
        /// <summary>
        /// Retrieves deposits for the indicated blocks from the block repository and throws an error if the blocks are not mature enough.
        /// </summary>
        /// <param name="blockHeight">The block height at which to start retrieving blocks.</param>
        /// <param name="maxBlocks">The number of blocks to retrieve.</param>
        /// <returns>A list of mature block deposits.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the blocks are not mature or not found.</exception>
        Task<Result<List<MaturedBlockDepositsModel>>> GetMaturedDepositsAsync(int blockHeight, int maxBlocks);
    }

    public class MaturedBlocksProvider : IMaturedBlocksProvider
    {
        private readonly IDepositExtractor depositExtractor;

        private readonly IConsensusManager consensusManager;

        private readonly ILogger logger;

        public MaturedBlocksProvider(ILoggerFactory loggerFactory, IDepositExtractor depositExtractor, IConsensusManager consensusManager)
        {
            this.depositExtractor = depositExtractor;
            this.consensusManager = consensusManager;

            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <inheritdoc />
        public async Task<Result<List<MaturedBlockDepositsModel>>> GetMaturedDepositsAsync(int blockHeight, int maxBlocks)
        {
            ChainedHeader consensusTip = this.consensusManager.Tip;

            int matureTipHeight = (consensusTip.Height - (int)this.depositExtractor.MinimumDepositConfirmations);

            if (blockHeight > matureTipHeight)
            {
                // We need to return a Result type here to explicitly indicate failure and the reason for failure.
                // This is an expected condition so we can avoid throwing an exception here.
                return Result<List<MaturedBlockDepositsModel>>.Fail($"Block height {blockHeight} submitted is not mature enough. Blocks less than a height of {matureTipHeight} can be processed.");
            }

            var maturedBlocks = new List<MaturedBlockDepositsModel>();

            // Half of the timeout. We will also need time to convert it to json.
            int maxTimeCollectionCanTakeMs = RestApiClientBase.TimeoutMs / 2;
            var cancellation = new CancellationTokenSource(maxTimeCollectionCanTakeMs);

            for (int i = blockHeight; (i <= matureTipHeight) && (i < blockHeight + maxBlocks); i++)
            {
                ChainedHeader currentHeader = consensusTip.GetAncestor(i);

                ChainedHeaderBlock block = this.consensusManager.GetBlockData(currentHeader.HashBlock);

                if (block?.Block?.Transactions == null)
                {
                    // Report unexpected results from consenus manager.
                    this.logger.LogWarning("Stop matured blocks collection due to consensus manager integrity failure. Send what we've collected.");
                    break;
                }

                MaturedBlockDepositsModel maturedBlockDeposits = this.depositExtractor.ExtractBlockDeposits(block);

                if (maturedBlockDeposits == null)
                    throw new InvalidOperationException($"Unable to get deposits for block at height {currentHeader.Height}");

                maturedBlocks.Add(maturedBlockDeposits);

                if (cancellation.IsCancellationRequested && maturedBlocks.Count > 0)
                {
                    this.logger.LogDebug("Stop matured blocks collection because it's taking too long. Send what we've collected.");
                    break;
                }
            }

            return Result<List<MaturedBlockDepositsModel>>.Ok(maturedBlocks);
        }
    }
}