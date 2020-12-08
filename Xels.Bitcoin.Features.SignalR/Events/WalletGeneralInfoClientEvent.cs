using System;
using System.Collections.Generic;
using Xels.Bitcoin.EventBus;
using Xels.Bitcoin.Features.Wallet.Models;

namespace Xels.Bitcoin.Features.SignalR.Events
{
    /// <summary>
    /// Marker type for Client
    /// </summary>
    public class WalletGeneralInfo
    {
    }

    public class WalletGeneralInfoClientEvent : WalletGeneralInfoModel, IClientEvent
    {
        public WalletGeneralInfoClientEvent(WalletGeneralInfoModel generalInfoModel)
        {
            this.WalletName = generalInfoModel.WalletName;
            this.Network = generalInfoModel.Network;
            this.ChainTip = generalInfoModel.ChainTip;
            this.ConnectedNodes = generalInfoModel.ConnectedNodes;
            this.CreationTime = generalInfoModel.CreationTime;
            this.IsDecrypted = generalInfoModel.IsDecrypted;
            this.IsChainSynced = generalInfoModel.IsChainSynced;
            this.LastBlockSyncedHeight = generalInfoModel.LastBlockSyncedHeight;
        }
        public Type NodeEventType => typeof(WalletGeneralInfo);
        
        public IEnumerable<AccountBalanceModel> AccountsBalances { get; set; }

        public void BuildFrom(EventBase @event)
        {
            throw new NotImplementedException();
        }
    }
}