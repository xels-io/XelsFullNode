﻿using Newtonsoft.Json;
using Xels.Features.FederatedPeg.Interfaces;
using Xels.Features.FederatedPeg.TargetChain;

namespace Xels.Features.FederatedPeg.Models
{
    public class WithdrawalModel
    {
        [JsonConverter(typeof(ConcreteConverter<Withdrawal>))]
        public IWithdrawal withdrawal { get; set; }

        public string SpendingOutputDetails { get; set; }

        public string TransferStatus { get; set; }

        public override string ToString()
        {
            return this.withdrawal.GetInfo() + " Spending=" + this.SpendingOutputDetails + " Status=" + this.TransferStatus;
        }
    }
}