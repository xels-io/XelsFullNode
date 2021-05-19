using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Xels.Bitcoin.Features.BlockStore;

namespace Xels.Bitcoin.Features.BlockExplorer
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockExplorerController : ControllerBase
    {
        private readonly ChainIndexer chain;
        private readonly IBlockRepository blockStoreCache;
        private readonly string network;

        public BlockExplorerController(ChainIndexer chain, IBlockRepository blockStoreCache)
        {
            this.chain = chain;
            this.blockStoreCache = blockStoreCache;
            this.network = this.chain.Network.ToString();
        }

        [Route("GetBlockInfo")]
        [HttpGet]
        public IActionResult GetBlockInfo(int height)
        {
            ChainedHeader chainedBlock = this.chain.GetHeader(height);

            Block block = this.blockStoreCache.GetBlock(chainedBlock.HashBlock);

            //Neo: Find out alternate work around.
            while (block == null)
            {
                block = this.blockStoreCache.GetBlock(chainedBlock.HashBlock);
                System.Threading.Thread.Sleep(200);
            }
            BlockInformation blockInfo = new BlockInformation(chainedBlock.Header);
            foreach (Transaction tx in block.Transactions)
            {
                //if (!tx.IsCoinStake)
                //{
                    //if (tx.Outputs.Count > 1)
                    //{
                    //    tx.Outputs.RemoveAt(0);
                    //}
                    blockInfo.Transactions.Add(new TransactionInfo(tx.GetHash(), tx.Version, tx.IsCoinStake, tx.Time, tx.Inputs, tx.Outputs, tx.LockTime, this.network));
                    blockInfo.TotalAmount += tx.TotalOut;
               // }
            }
            blockInfo.blockSize = block.BlockSize;
            //blockInfo.BlockReward = block.Transactions[0].TotalOut + block.Transactions[1].TotalOut;
            blockInfo.BlockReward = Money.Coins( 50 ); //block.Transactions[0].TotalOut == 0 ? block.Transactions[1].TotalOut : block.Transactions[0].TotalOut;
            blockInfo.TransactionCount = block.Transactions.Count;
            blockInfo.BlockData = block;
            blockInfo.BlockId = chainedBlock.HashBlock;
            blockInfo.Confirmations = this.chain.Tip.Height - chainedBlock.Height + 1;
            blockInfo.Height = chainedBlock.Height;

            return this.Ok(blockInfo);
        }

        [Route("GetAllBlockInfo")]
        [HttpGet]
        public IActionResult GetAllBlockInfo()
        {
            //Dictionary<int, ChainedHeader>.ValueCollection lstChainedBlocks = this.chain.blocksByHeight.Values;
            List<BlockInformation> lstBlockInformation = new List<BlockInformation>();
            foreach (ChainedHeader chainedBlock in this.chain.blocksByHeight.Values)
            {
                Block block = this.blockStoreCache.GetBlock(chainedBlock.HashBlock);
                BlockInformation blockInfo = new BlockInformation(chainedBlock.Header);
                if (block != null)
                {
                    // blockInfo.Transactions = block.Transactions;
                    foreach (Transaction tx in block.Transactions)
                    {
                        blockInfo.Transactions.Add(new TransactionInfo( tx.GetHash(), tx.Version, tx.IsCoinStake, tx.Time, tx.Inputs, tx.Outputs, tx.LockTime, this.network));
                        blockInfo.TotalAmount += tx.TotalOut;
                        blockInfo.BlockReward = Money.Coins(50); //block.Transactions[0].TotalOut;
                      
                    }
                    blockInfo.TransactionCount = block.Transactions.Count;
                    blockInfo.BlockData = block;
                }
                blockInfo.BlockId = chainedBlock.HashBlock;
                blockInfo.Confirmations = this.chain.Tip.Height - chainedBlock.Height + 1;
                blockInfo.Height = chainedBlock.Height;
                lstBlockInformation.Add(blockInfo);
            }
            return this.Ok(lstBlockInformation);
        }

        [Route("RestblockAppend")]
        [HttpGet]
        public IActionResult RestBlocks(int height)
        {
            List<ChainedHeader> lstChainedBlocks = this.chain.GetRestBlocks(height);

            List<BlockInformation> lstBlockInformation = new List<BlockInformation>();
            foreach (ChainedHeader chainedBlock in lstChainedBlocks)
            {
                Block block = this.blockStoreCache.GetBlock(chainedBlock.HashBlock);
                BlockInformation blockInfo = new BlockInformation(chainedBlock.Header);

                if (block != null)
                {
                    foreach (Transaction tx in block.Transactions)
                    {
                        blockInfo.Transactions.Add(new TransactionInfo(tx.GetHash(), tx.Version, tx.IsCoinStake, tx.Time, tx.Inputs, tx.Outputs, tx.LockTime, this.network));
                        blockInfo.TotalAmount += tx.TotalOut;
                    }
                    blockInfo.blockSize = block.BlockSize;
                    blockInfo.BlockReward = Money.Coins(50); //block.Transactions[0].TotalOut;  // == 0 ? block.Transactions[1].TotalOut : block.Transactions[0].TotalOut;
                    blockInfo.TransactionCount = block.Transactions.Count;
                    blockInfo.BlockData = block;
                }

                blockInfo.BlockId = chainedBlock.HashBlock;
                blockInfo.Confirmations = this.chain.Tip.Height - chainedBlock.Height + 1;
                blockInfo.Height = chainedBlock.Height;
                lstBlockInformation.Add(blockInfo);
            }

            return this.Ok(lstBlockInformation);
        }
      
        [Route("GetLastNBlockInfo")]
        [HttpGet]
        public IActionResult GetLastNBlockInfo(int numberOfBlocks)
        {
            List<ChainedHeader> lstChainedBlocks = this.chain.GetLastNBlocksInfo(numberOfBlocks);
            List<BlockInformation> lstBlockInformation = new List<BlockInformation>();

            foreach (ChainedHeader chainedBlock in lstChainedBlocks)
            {
                Block block = this.blockStoreCache.GetBlock(chainedBlock.HashBlock);
                BlockInformation blockInfo = new BlockInformation(chainedBlock.Header);
                //BlockInformation PrevBlockInfo = new BlockInformation(chainedBlock.Previous);

                if (block != null)
                {
                    
                    //blockInfo.Transactions = block.Transactions;
                    foreach (Transaction tx in block.Transactions)
                    {
                        // if (!tx.IsCoinStake)
                        //{
                      
                        blockInfo.Transactions.Add(new TransactionInfo(tx.GetHash(), tx.Version, tx.IsCoinStake, tx.Time, tx.Inputs, tx.Outputs, tx.LockTime, this.network));
                        blockInfo.TotalAmount += tx.TotalOut;
                        blockInfo.BlockReward = Money.Coins(50); //block.Transactions[0].TotalOut;
                        //}
                    }
                  //  blockInfo.Previous = chainedBlock.Previous;


                    blockInfo.TransactionCount = block.Transactions.Count;
                    blockInfo.BlockData = block;
                }
                blockInfo.BlockId = chainedBlock.HashBlock;
                blockInfo.Confirmations = this.chain.Tip.Height - chainedBlock.Height + 1;
                blockInfo.Height = chainedBlock.Height;
                lstBlockInformation.Add(blockInfo);
            }
            return this.Ok(lstBlockInformation);
        }

        //[Route("GetTransaction")]
        //[HttpGet]
        //public IActionResult GetTransaction(string strTxId)
        //{
        //    RPCClient rpcClient = GetRPC();
        //    Transaction tx = rpcClient.GetRawTransaction(uint256.Parse(strTxId), false);

        //    return this.Ok(tx);
        //}

        //public static RPCClient GetRPC()
        //{
        //    var credentials = new System.Net.NetworkCredential("user", "pass");
        //    return new RPCClient(credentials, new Uri("http://127.0.0.1:19775/"), Network.XelsMain);
        //}
    }
}
