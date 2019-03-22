using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;

namespace Xels.Bitcoin.Features.BlockExplorer
{
    public class BlockInformation
    {
        public BlockInformation()
        {

        }
        public BlockInformation(BlockHeader header)
        {
            BlockId = header.GetHash();
            BlockHeader = header;
            BlockTime = header.BlockTime.DateTime;
            Height = -1;
            Confirmations = -1;
            BlockData = new Block();
            blockSize = (long)0;
            TransactionCount = 0;
            TotalAmount = Money.Zero;
            BlockReward = Money.Zero;
            Transactions = new List<TransactionInfo>();
        }

        public List<TransactionInfo> Transactions
        {
            get; set;
        }
        public long? blockSize
        {
            get;set;
        }
        public Money BlockReward
        {
            get; set;
        }
       
        public Money TotalAmount
        {
            get; set;
        }

        public int TransactionCount
        {
            get; set;
        }

        public Block BlockData
        {
            get; set;
        }

        public uint256 BlockId
        {
            get;
            set;
        }

        public BlockHeader BlockHeader
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public int Confirmations
        {
            get;
            set;
        }

        public DateTimeOffset MedianTimePast
        {
            get;
            set;
        }

        public DateTime BlockTime
        {
            get;
            set;
        }
    }
}