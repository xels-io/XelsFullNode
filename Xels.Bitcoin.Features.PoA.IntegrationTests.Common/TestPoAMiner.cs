using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.AsyncWork;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Validators;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.PoA.Voting;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using Xels.Bitcoin.Interfaces;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.PoA.IntegrationTests.Common
{
    public class TestPoAMiner : PoAMiner
    {
        private readonly EditableTimeProvider timeProvider;

        private readonly CancellationTokenSource cancellation;

        private readonly ISlotsManager slotsManager;

        private readonly IConsensusManager consensusManager;

        public TestPoAMiner(
            IConsensusManager consensusManager,
            IDateTimeProvider dateTimeProvider,
            Network network,
            INodeLifetime nodeLifetime,
            ILoggerFactory loggerFactory,
            IInitialBlockDownloadState ibdState,
            BlockDefinition blockDefinition,
            ISlotsManager slotsManager,
            IConnectionManager connectionManager,
            PoABlockHeaderValidator poaHeaderValidator,
            IFederationManager federationManager,
            IIntegrityValidator integrityValidator,
            IWalletManager walletManager,
            INodeStats nodeStats,
            VotingManager votingManager,
            PoAMinerSettings poAMinerSettings,
            IAsyncProvider asyncProvider) : base(consensusManager, dateTimeProvider, network, nodeLifetime, loggerFactory, ibdState, blockDefinition, slotsManager,
                connectionManager, poaHeaderValidator, federationManager, integrityValidator, walletManager, nodeStats, votingManager, poAMinerSettings, asyncProvider)
        {
            this.timeProvider = dateTimeProvider as EditableTimeProvider;

            this.cancellation = new CancellationTokenSource();
            this.slotsManager = slotsManager;
            this.consensusManager = consensusManager;
        }

        public override void InitializeMining()
        {
        }

        public async Task MineBlocksAsync(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.timeProvider.AdjustedTimeOffset += TimeSpan.FromSeconds(
                    this.slotsManager.GetRoundLengthSeconds(this.federationManager.GetFederationMembers().Count));

                uint timeNow = (uint)this.timeProvider.GetAdjustedTimeAsUnixTimestamp();

                uint myTimestamp = this.slotsManager.GetMiningTimestamp(timeNow);

                this.timeProvider.AdjustedTimeOffset += TimeSpan.FromSeconds(myTimestamp - timeNow);

                ChainedHeader chainedHeader = await this.MineBlockAtTimestampAsync(myTimestamp).ConfigureAwait(false);

                if (chainedHeader == null)
                {
                    i--;
                    this.timeProvider.AdjustedTimeOffset += TimeSpan.FromHours(1);
                    continue;
                }

                var builder = new StringBuilder();
                builder.AppendLine("<<==============================================================>>");
                builder.AppendLine($"Block was mined {chainedHeader}.");
                builder.AppendLine("<<==============================================================>>");
                this.logger.LogInformation(builder.ToString());
            }
        }

        public override void Dispose()
        {
            this.cancellation.Cancel();
            base.Dispose();
        }
    }
}
