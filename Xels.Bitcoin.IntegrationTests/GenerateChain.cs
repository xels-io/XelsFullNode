using System.IO;
using System.IO.Compression;
using NBitcoin;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Networks;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests.Common
{
    public class GenerateChain
    {
        private const string SkipTestMessage = "Not to be run as part of the unit test suite.";

        private const string MinerMnemonic = "elevator slight dad hair table forum maze feed trim ignore field mystery";
        private const string ListenerMnemonic = "seminar cool use bleak drink section rent bid language obey skin round";
        private const string DataPath = @"..\..\..\..\Xels.Bitcoin.IntegrationTests.Common\ReadyData";

        public GenerateChain()
        {
            Directory.CreateDirectory(DataPath);
        }

        [Fact(Skip = SkipTestMessage)]
        public void CreateXelsBlockchainDataWith10Blocks()
        {
            this.GenerateXelsBlockchainData(new XelsRegTest(), 10, true, true, true);
        }

        [Fact(Skip = SkipTestMessage)]
        public void CreateXelsBlockchainDataWith100Blocks()
        {
            this.GenerateXelsBlockchainData(new XelsRegTest(), 100, true, true, true);
        }

        [Fact(Skip = SkipTestMessage)]
        public void CreateXelsBlockchainDataWith150Blocks()
        {
            this.GenerateXelsBlockchainData(new XelsRegTest(), 150, true, true, true);
        }

        [Fact(Skip = SkipTestMessage)]
        public void CreateBitcoinBlockchainDataWith10Blocks()
        {
            this.GenerateBitcoinBlockchainData(new BitcoinRegTest(), 10, true, true, true);
        }

        [Fact(Skip = SkipTestMessage)]
        public void CreateBitcoinBlockchainDataWith100Blocks()
        {
            this.GenerateBitcoinBlockchainData(new BitcoinRegTest(), 100, true, true, true);
        }

        [Fact(Skip = SkipTestMessage)]
        public void CreateBitcoinBlockchainDataWith150Blocks()
        {
            this.GenerateBitcoinBlockchainData(new BitcoinRegTest(), 150, true, true, true);
        }

        private void GenerateXelsBlockchainData(Network network, int blockCount, bool saveMinerFolderWithWallet, bool saveListenerFolderWithSyncedEmptyWallet, bool saveFolderWithoutWallet)
        {
            string dataFolderPath, listenerFolderPath;

            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                // Arrange.
                // Create a sending and a receiving node.
                CoreNode miningNode = builder.CreateXelsPosNode(network).WithWallet(walletMnemonic: MinerMnemonic).Start();
                CoreNode listeningNode = builder.CreateXelsPosNode(network).WithWallet(walletMnemonic: ListenerMnemonic).Start();

                TestHelper.Connect(miningNode, listeningNode);
                TestHelper.MineBlocks(miningNode, blockCount);
                TestHelper.WaitForNodeToSync(miningNode, listeningNode);

                dataFolderPath = miningNode.DataFolder;
                listenerFolderPath = listeningNode.DataFolder;
            }

            if (saveMinerFolderWithWallet)
            {
                File.Delete(Path.Combine(dataFolderPath, "xels.conf"));
                ZipDataFolder(dataFolderPath, $"{network.Name}{blockCount}Miner.zip", DataPath);
            }
            
            if (saveListenerFolderWithSyncedEmptyWallet)
            {
                File.Delete(Path.Combine(listenerFolderPath, "xels.conf"));
                ZipDataFolder(listenerFolderPath, $"{network.Name}{blockCount}Listener.zip", DataPath);
            }

            if (saveFolderWithoutWallet)
            {
                foreach (string walletFile in Directory.EnumerateFiles(dataFolderPath, "*.wallet.json", SearchOption.AllDirectories))
                {
                    File.Delete(walletFile);
                }

                ZipDataFolder(dataFolderPath, $"{network.Name}{blockCount}NoWallet.zip", DataPath);
            }
        }

        private void GenerateBitcoinBlockchainData(Network network, int blockCount, bool saveMinerFolderWithWallet, bool saveListenerFolderWithSyncedEmptyWallet, bool saveFolderWithoutWallet)
        {
            string dataFolderPath, listenerFolderPath;

            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                // Arrange.
                // Create a sending and a receiving node.
                CoreNode miningNode = builder.CreateXelsPowNode(network).WithWallet(walletMnemonic: MinerMnemonic).Start();
                CoreNode listeningNode = builder.CreateXelsPowNode(network).WithWallet(walletMnemonic: ListenerMnemonic).Start();

                TestHelper.Connect(miningNode, listeningNode);
                TestHelper.MineBlocks(miningNode, blockCount);
                TestHelper.WaitForNodeToSync(miningNode, listeningNode);

                dataFolderPath = miningNode.DataFolder;
                listenerFolderPath = listeningNode.DataFolder;
            }

            if (saveMinerFolderWithWallet)
            {
                File.Delete(Path.Combine(dataFolderPath, "bitcoin.conf"));
                ZipDataFolder(dataFolderPath, $"{network.Name}{blockCount}Miner.zip", DataPath);
            }

            if (saveListenerFolderWithSyncedEmptyWallet)
            {
                File.Delete(Path.Combine(listenerFolderPath, "bitcoin.conf"));
                ZipDataFolder(listenerFolderPath, $"{network.Name}{blockCount}Listener.zip", DataPath);
            }

            if (saveFolderWithoutWallet)
            {
                foreach (string walletFile in Directory.EnumerateFiles(dataFolderPath, "*.wallet.json", SearchOption.AllDirectories))
                {
                    File.Delete(walletFile);
                }

                ZipDataFolder(dataFolderPath, $"{network.Name}{blockCount}NoWallet.zip", DataPath);
            }
        }

        private static void ZipDataFolder(string folderToZip, string zipFileName, string destinationFolder)
        {
            string zipPath = Path.Combine(destinationFolder, zipFileName);

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(Path.GetFullPath(folderToZip), zipPath);
        }
    }
}
