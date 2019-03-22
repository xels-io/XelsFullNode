using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Xels.Bitcoin.Features.BlockStore;

namespace Xels.Bitcoin.Features.BlockExplorer
{
    [Route("api/[controller]")]
    public class BlockExplorerController : Controller
    {
        private readonly ConcurrentChain chain;
        private readonly IBlockRepository blockStoreCache;

        public BlockExplorerController(ConcurrentChain chain, IBlockRepository blockStoreCache)
        {
            this.chain = chain;
            this.blockStoreCache = blockStoreCache;
        }

        [Route("GetBlockInfo")]
        [HttpGet]
        public IActionResult GetBlockInfo(int height)
        {
            ChainedHeader chainedBlock = this.chain.GetBlock(height);

            Block block = this.blockStoreCache.GetBlockAsync(chainedBlock.HashBlock).GetAwaiter().GetResult();

            //Neo: Find out alternate work around.
            while (block == null)
            {
                block = this.blockStoreCache.GetBlockAsync(chainedBlock.HashBlock).GetAwaiter().GetResult();
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
                    blockInfo.Transactions.Add(new TransactionInfo(tx.GetHash(), tx.Version, tx.IsCoinStake, tx.Time, tx.vin, tx.vout, tx.nLockTime));
                    blockInfo.TotalAmount += tx.TotalOut;
                    blockInfo.BlockReward = block.Transactions[0].TotalOut;
               // }
            }
            blockInfo.TransactionCount = block.Transactions.Count;
            blockInfo.BlockData = block;
            blockInfo.BlockId = chainedBlock.HashBlock;
            blockInfo.Confirmations = this.chain.Tip.Height - chainedBlock.Height + 1;
            blockInfo.Height = chainedBlock.Height;

            return this.Json(blockInfo);
        }

        [Route("GetAllBlockInfo")]
        [HttpGet]
        public IActionResult GetAllBlockInfo()
        {
            List<ChainedHeader> lstChainedBlocks = this.chain.GetAllBlocks();
            List<BlockInformation> lstBlockInformation = new List<BlockInformation>();
            foreach (ChainedHeader chainedBlock in lstChainedBlocks)
            {
                Block block = this.blockStoreCache.GetBlockAsync(chainedBlock.HashBlock).GetAwaiter().GetResult();
                BlockInformation blockInfo = new BlockInformation(chainedBlock.Header);

                if (block != null)
                {
                    // blockInfo.Transactions = block.Transactions;
                    foreach (Transaction tx in block.Transactions)
                    {
                        blockInfo.Transactions.Add(new TransactionInfo(tx.GetHash(), tx.Version, tx.IsCoinStake, tx.Time, tx.vin, tx.vout, tx.nLockTime));
                        blockInfo.TotalAmount += tx.TotalOut;
                        blockInfo.BlockReward = block.Transactions[0].TotalOut;
                      
                    }
                    blockInfo.TransactionCount = block.Transactions.Count;
                    blockInfo.BlockData = block;
                }
                blockInfo.BlockId = chainedBlock.HashBlock;
                blockInfo.Confirmations = this.chain.Tip.Height - chainedBlock.Height + 1;
                blockInfo.Height = chainedBlock.Height;
                lstBlockInformation.Add(blockInfo);
            }
            return this.Json(lstBlockInformation);
        }

        [Route("RestblockAppend")]
        [HttpGet]
        public IActionResult RestBlocks(int height)
        {
            List<ChainedHeader> lstChainedBlocks = this.chain.GetRestBlocks(height);
            List<BlockInformation> lstBlockInformation = new List<BlockInformation>();
            foreach (ChainedHeader chainedBlock in lstChainedBlocks)
            {
                Block block = this.blockStoreCache.GetBlockAsync(chainedBlock.HashBlock).GetAwaiter().GetResult();
                BlockInformation blockInfo = new BlockInformation(chainedBlock.Header);
              
                if (block != null)
                {
                    foreach (Transaction tx in block.Transactions)
                    {
                        blockInfo.Transactions.Add(new TransactionInfo(tx.GetHash(), tx.Version, tx.IsCoinStake, tx.Time, tx.vin, tx.vout, tx.nLockTime));
                        blockInfo.TotalAmount += tx.TotalOut;
                        blockInfo.BlockReward = block.Transactions[0].TotalOut;
                        blockInfo.blockSize = block.BlockSize;
                        
                    }
                    blockInfo.TransactionCount = block.Transactions.Count;
                    blockInfo.BlockData = block;
                }

                blockInfo.BlockId = chainedBlock.HashBlock;
                blockInfo.Confirmations = this.chain.Tip.Height - chainedBlock.Height + 1;
                blockInfo.Height = chainedBlock.Height;
                lstBlockInformation.Add(blockInfo);
            }

            return this.Json(lstBlockInformation);
        }
      
        [Route("GetLastNBlockInfo")]
        [HttpGet]
        public IActionResult GetLastNBlockInfo(int numberOfBlocks)
        {
            List<ChainedHeader> lstChainedBlocks = this.chain.GetLastNBlocksInfo(numberOfBlocks);
            List<BlockInformation> lstBlockInformation = new List<BlockInformation>();

            foreach (ChainedHeader chainedBlock in lstChainedBlocks)
            {
                Block block = this.blockStoreCache.GetBlockAsync(chainedBlock.HashBlock).GetAwaiter().GetResult();
                BlockInformation blockInfo = new BlockInformation(chainedBlock.Header);
                //BlockInformation PrevBlockInfo = new BlockInformation(chainedBlock.Previous);

                if (block != null)
                {
                    
                    //blockInfo.Transactions = block.Transactions;
                    foreach (Transaction tx in block.Transactions)
                    {
                        // if (!tx.IsCoinStake)
                        //{
                      
                        blockInfo.Transactions.Add(new TransactionInfo(tx.GetHash(), tx.Version, tx.IsCoinStake, tx.Time, tx.vin, tx.vout, tx.nLockTime));
                        blockInfo.TotalAmount += tx.TotalOut;
                        blockInfo.BlockReward = block.Transactions[0].TotalOut;
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
            return this.Json(lstBlockInformation);
        }

        //[Route("GetTransaction")]
        //[HttpGet]
        //public IActionResult GetTransaction(string strTxId)
        //{
        //    RPCClient rpcClient = GetRPC();
        //    Transaction tx = rpcClient.GetRawTransaction(uint256.Parse(strTxId), false);

        //    return this.Json(tx);
        //}

        //public static RPCClient GetRPC()
        //{
        //    var credentials = new System.Net.NetworkCredential("user", "pass");
        //    return new RPCClient(credentials, new Uri("http://127.0.0.1:19775/"), Network.XelsMain);
        //}
    }
}
