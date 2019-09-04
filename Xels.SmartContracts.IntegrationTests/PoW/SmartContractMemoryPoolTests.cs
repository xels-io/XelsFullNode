using System.Linq;
using System.Threading;
using NBitcoin;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.Tests.Common;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.CLR.Serialization;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Tests.Common;
using Xunit;

namespace Xels.SmartContracts.IntegrationTests.PoW
{
    public class SmartContractMemoryPoolTests
    {
        [Fact]
        public void SmartContracts_AddToMempool_Success()
        {
            using (SmartContractNodeBuilder builder = SmartContractNodeBuilder.Create(this))
            {
                var xelsNodeSync = builder.CreateSmartContractPowNode().WithWallet().Start();

                TestHelper.MineBlocks(xelsNodeSync, 105); // coinbase maturity = 100

                var block = xelsNodeSync.FullNode.BlockStore().GetBlock(xelsNodeSync.FullNode.ChainIndexer.GetHeader(4).HashBlock);
                var prevTrx = block.Transactions.First();
                var dest = new BitcoinSecret(new Key(), xelsNodeSync.FullNode.Network);

                var tx = xelsNodeSync.FullNode.Network.CreateTransaction();
                tx.AddInput(new TxIn(new OutPoint(prevTrx.GetHash(), 0), PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(xelsNodeSync.MinerSecret.PubKey)));
                tx.AddOutput(new TxOut("25", dest.PubKey.Hash));
                tx.AddOutput(new TxOut("24", new Key().PubKey.Hash)); // 1 btc fee
                tx.Sign(xelsNodeSync.FullNode.Network, xelsNodeSync.MinerSecret, false);

                xelsNodeSync.Broadcast(tx);

                TestBase.WaitLoop(() => xelsNodeSync.CreateRPCClient().GetRawMempool().Length == 1);
            }
        }

        [Fact]
        public void SmartContracts_AddToMempool_OnlyValid()
        {
            using (SmartContractNodeBuilder builder = SmartContractNodeBuilder.Create(this))
            {
                var xelsNodeSync = builder.CreateSmartContractPowNode().WithWallet().Start();

                var callDataSerializer = new CallDataSerializer(new ContractPrimitiveSerializer(xelsNodeSync.FullNode.Network));

                TestHelper.MineBlocks(xelsNodeSync, 105); // coinbase maturity = 100

                var block = xelsNodeSync.FullNode.BlockStore().GetBlock(xelsNodeSync.FullNode.ChainIndexer.GetHeader(4).HashBlock);
                var prevTrx = block.Transactions.First();
                var dest = new BitcoinSecret(new Key(), xelsNodeSync.FullNode.Network);

                // Gas higher than allowed limit
                var tx = xelsNodeSync.FullNode.Network.CreateTransaction();
                tx.AddInput(new TxIn(new OutPoint(prevTrx.GetHash(), 0), PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(xelsNodeSync.MinerSecret.PubKey)));
                var contractTxData = new ContractTxData(1, 100, new RuntimeObserver.Gas(10_000_000), new uint160(0), "Test");
                tx.AddOutput(new TxOut(1, new Script(callDataSerializer.Serialize(contractTxData))));
                tx.Sign(xelsNodeSync.FullNode.Network, xelsNodeSync.MinerSecret, false);
                xelsNodeSync.Broadcast(tx);

                // OP_SPEND in user's tx - we can't sign this because the TransactionBuilder recognises the ScriptPubKey is invalid.
                tx = xelsNodeSync.FullNode.Network.CreateTransaction();
                tx.AddInput(new TxIn(new OutPoint(prevTrx.GetHash(), 0), new Script(new[] { (byte)ScOpcodeType.OP_SPEND })));
                tx.AddOutput(new TxOut(1, new Script(callDataSerializer.Serialize(contractTxData))));
                xelsNodeSync.Broadcast(tx);

                // 2 smart contract outputs
                tx = xelsNodeSync.FullNode.Network.CreateTransaction();
                tx.AddInput(new TxIn(new OutPoint(prevTrx.GetHash(), 0), PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(xelsNodeSync.MinerSecret.PubKey)));
                tx.AddOutput(new TxOut(1, new Script(callDataSerializer.Serialize(contractTxData))));
                tx.AddOutput(new TxOut(1, new Script(callDataSerializer.Serialize(contractTxData))));
                tx.Sign(xelsNodeSync.FullNode.Network, xelsNodeSync.MinerSecret, false);
                xelsNodeSync.Broadcast(tx);

                // Send to contract
                uint160 contractAddress = new uint160(123);
                var state = xelsNodeSync.FullNode.NodeService<IStateRepositoryRoot>();
                state.CreateAccount(contractAddress);
                state.Commit();
                tx = xelsNodeSync.FullNode.Network.CreateTransaction();
                tx.AddInput(new TxIn(new OutPoint(prevTrx.GetHash(), 0), PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(xelsNodeSync.MinerSecret.PubKey)));
                tx.AddOutput(new TxOut(100, PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(new KeyId(contractAddress))));
                tx.Sign(xelsNodeSync.FullNode.Network, xelsNodeSync.MinerSecret, false);
                xelsNodeSync.Broadcast(tx);

                // Gas price lower than minimum
                tx = xelsNodeSync.FullNode.Network.CreateTransaction();
                tx.AddInput(new TxIn(new OutPoint(prevTrx.GetHash(), 0), PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(xelsNodeSync.MinerSecret.PubKey)));
                var lowGasPriceContractTxData = new ContractTxData(1, SmartContractMempoolValidator.MinGasPrice - 1, new RuntimeObserver.Gas(SmartContractFormatLogic.GasLimitMaximum), new uint160(0), "Test");
                tx.AddOutput(new TxOut(1, new Script(callDataSerializer.Serialize(lowGasPriceContractTxData))));
                tx.Sign(xelsNodeSync.FullNode.Network, xelsNodeSync.MinerSecret, false);
                xelsNodeSync.Broadcast(tx);

                // After 5 seconds (plenty of time but ideally we would have a more accurate measure) no txs in mempool. All failed validation.
                Thread.Sleep(5000);
                Assert.Empty(xelsNodeSync.CreateRPCClient().GetRawMempool());

                // Valid tx still works
                tx = xelsNodeSync.FullNode.Network.CreateTransaction();
                tx.AddInput(new TxIn(new OutPoint(prevTrx.GetHash(), 0), PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(xelsNodeSync.MinerSecret.PubKey)));
                tx.AddOutput(new TxOut("25", dest.PubKey.Hash));
                tx.AddOutput(new TxOut("24", new Key().PubKey.Hash)); // 1 btc fee
                tx.Sign(xelsNodeSync.FullNode.Network, xelsNodeSync.MinerSecret, false);
                xelsNodeSync.Broadcast(tx);
                TestBase.WaitLoop(() => xelsNodeSync.CreateRPCClient().GetRawMempool().Length == 1);
            }
        }
    }
}