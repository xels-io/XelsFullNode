using System.Windows;

using NBitcoin;

using Xels.Bitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Controllers.Models;
using Xels.Bitcoin.Utilities;

namespace XellsFrontEnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //ChainIndexer chainIndexer;
        private readonly IFullNode _fullNode;
        private readonly IChainState chainState;
        //private readonly NodeSettings nodeSettings;
        //private readonly Network network;
        private readonly IDateTimeProvider dateTimeProvider;

        public MainWindow(/*ChainIndexer chainIndexer,*/ IFullNode fullNode, IChainState chainState, /*NodeSettings nodeSettings, Network network,*/ IDateTimeProvider dateTimeProvider)
        {
           

            this._fullNode = fullNode;
            this.chainState = chainState;
            //this.nodeSettings = nodeSettings;
            //this.network = network;
            this.dateTimeProvider = dateTimeProvider;

            InitializeComponent();
            Status();
        }

        //public MainWindow()
        //{
        //    //InitializeComponent();
        //    //Status();
        //}



        public StatusModel Status()
        {
            var model = new StatusModel();

            model.Version = this._fullNode.Version?.ToString() ?? "0";
            //model.Network = this._fullNode.Network.Name;
            model.ConsensusHeight = this.chainState.ConsensusTip?.Height;

            //model.RelayFee = this.nodeSettings.MinRelayTxFeeRate?.FeePerK?.ToUnit(MoneyUnit.BTC) ?? 0;
            model.RunningTime = this.dateTimeProvider.GetUtcNow() - this._fullNode.StartTime;
            //model.CoinTicker = this.network.CoinTicker;
            model.State = this._fullNode.State.ToString();

            return model;

        }

        //private void button_Click(object sender, RoutedEventArgs e)
        //{
        //    Status();
        //}
    }
}
