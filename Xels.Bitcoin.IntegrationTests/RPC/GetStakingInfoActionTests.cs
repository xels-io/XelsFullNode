using System.Threading.Tasks;
using Xels.Bitcoin.Features.Miner.Controllers;
using Xels.Bitcoin.Features.Miner.Interfaces;
using Xels.Bitcoin.Features.Miner.Models;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.Runners;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Utilities;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests.RPC
{
    /// <summary>
    /// Tests of RPC controller action "getstakinginfo".
    /// </summary>
    public class GetStakingInfoActionTests
    {
        /// <summary>
        /// Tests that the RPC controller of a staking node correctly replies to "getstakinginfo" command.
        /// </summary>
        [Fact]
        public void GetStakingInfo_StakingEnabled()
        {
            IFullNode fullNode = XelsBitcoinPosRunner.BuildStakingNode(TestBase.CreateTestDir(this));
            Task fullNodeRunTask = fullNode.RunAsync();

            var nodeLifetime = fullNode.NodeService<INodeLifetime>();
            nodeLifetime.ApplicationStarted.WaitHandle.WaitOne();
            var controller = fullNode.NodeController<StakingRpcController>();

            Assert.NotNull(fullNode.NodeService<IPosMinting>(true));

            GetStakingInfoModel info = controller.GetStakingInfo();

            Assert.NotNull(info);
            Assert.True(info.Enabled);
            Assert.False(info.Staking);

            nodeLifetime.StopApplication();
            nodeLifetime.ApplicationStopped.WaitHandle.WaitOne();
            fullNode.Dispose();

            Assert.False(fullNodeRunTask.IsFaulted);
        }

        /// <summary>
        /// Tests that the RPC controller of a staking node correctly replies to "startstaking" command.
        /// </summary>
        [Fact]
        public void GetStakingInfo_StartStaking()
        {
            IFullNode fullNode = XelsBitcoinPosRunner.BuildStakingNode(TestBase.CreateTestDir(this), false);
            var node = fullNode as FullNode;

            Task fullNodeRunTask = fullNode.RunAsync();

            var nodeLifetime = fullNode.NodeService<INodeLifetime>();
            nodeLifetime.ApplicationStarted.WaitHandle.WaitOne();
            var controller = fullNode.NodeController<StakingRpcController>();

            var walletManager = node.NodeService<IWalletManager>() as WalletManager;

            string password = "test";
            string passphrase = "passphrase";

            // create the wallet
            walletManager.CreateWallet(password, "test", passphrase);

            Assert.NotNull(fullNode.NodeService<IPosMinting>(true));

            GetStakingInfoModel info = controller.GetStakingInfo();

            Assert.NotNull(info);
            Assert.False(info.Enabled);
            Assert.False(info.Staking);

            controller.StartStaking("test", "test");

            info = controller.GetStakingInfo();

            Assert.NotNull(info);
            Assert.True(info.Enabled);
            Assert.False(info.Staking);

            nodeLifetime.StopApplication();
            nodeLifetime.ApplicationStopped.WaitHandle.WaitOne();
            fullNode.Dispose();

            Assert.False(fullNodeRunTask.IsFaulted);
        }
    }
}