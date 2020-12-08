using NBitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Configuration.Settings;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Interfaces;

namespace Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers
{
    public class InitialBlockDownloadStateMock : IInitialBlockDownloadState
    {
        public InitialBlockDownloadStateMock(IChainState chainState, Network network, ConsensusSettings consensusSettings, ICheckpoints checkpoints)
        {
        }

        public bool IsInitialBlockDownload()
        {
            return false;
        }
    }
}
