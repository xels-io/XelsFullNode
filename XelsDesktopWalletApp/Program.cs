using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IWshRuntimeLibrary;

using Microsoft.Extensions.Hosting;

using NBitcoin;
using NBitcoin.Protocol;

using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.ColdStaking;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.Notifications;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SignalR;
using Xels.Bitcoin.Features.SignalR.Broadcasters;
using Xels.Bitcoin.Features.SignalR.Events;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.Utilities;
using Xels.Features.Collateral;
using Xels.Features.Collateral.CounterChain;
using Xels.Features.Diagnostic;
using Xels.Features.SQLiteWalletRepository;
using Xels.Sidechains.Networks;

namespace XelsDesktopWalletApp
{
    public class Program
    {
        //   xels.xoyped.d ----start up 

        //private const string MainchainArgument = "-mainchain";
        //private const string SidechainArgument = "-sidechain";

        //private static readonly Dictionary<NetworkType, Func<Network>> SidechainNetworks = new Dictionary<NetworkType, Func<Network>>
        //{
        //    { NetworkType.Mainnet, XoyNetwork.NetworksSelector.Mainnet },
        //    { NetworkType.Testnet, XoyNetwork.NetworksSelector.Testnet },
        //    { NetworkType.Regtest, XoyNetwork.NetworksSelector.Regtest }
        //};

        //private static readonly Dictionary<NetworkType, Func<Network>> MainChainNetworks = new Dictionary<NetworkType, Func<Network>>
        //{
        //    { NetworkType.Mainnet, Networks.Xels.Mainnet },
        //    { NetworkType.Testnet, Networks.Xels.Testnet },
        //    { NetworkType.Regtest, Networks.Xels.Regtest }
        //};

        //[STAThread]
        //private static void Main(string[] args)
        //{
        //    args = new string[] { "-mainchain" };
        //    App application = new App();
        //    RunFederationGatewayAsync(args).Wait();
        //    //CreateShortCut();

        //    application.InitializeComponent();
        //    application.Run();
        //}

        //private static async Task RunFederationGatewayAsync(string[] args)
        //{
        //    try
        //    {
        //        bool isMainchainNode = args.FirstOrDefault(a => a.ToLower() == MainchainArgument) != null;
        //        bool isSidechainNode = args.FirstOrDefault(a => a.ToLower() == SidechainArgument) != null;

        //        if (isSidechainNode == isMainchainNode)
        //        {
        //            throw new ArgumentException($"Gateway node needs to be started specifying either a {SidechainArgument} or a {MainchainArgument} argument");
        //        }

        //        IFullNode node = isMainchainNode ? GetMainchainFullNode(args) : GetSidechainFullNode(args);

        //        if (node != null)
        //            await node.RunAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.Message);
        //    }
        //}

        //private static IFullNode GetMainchainFullNode(string[] args)
        //{
        //    // TODO: Hardcode -addressindex

        //    var nodeSettings = new NodeSettings(networksSelector: Networks.Xels, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: args)
        //    {
        //        MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
        //    };

        //    NetworkType networkType = nodeSettings.Network.NetworkType;

        //    var fedPegOptions = new FederatedPegOptions(
        //        walletSyncFromHeight: new int[] { FederatedPegSettings.XelsMainDepositStartBlock, 1, 1 }[(int)networkType]
        //    );

        //    IFullNode node = new FullNodeBuilder()
        //        .UseNodeSettings(nodeSettings)
        //        .UseBlockStore()
        //        .SetCounterChainNetwork(SidechainNetworks[nodeSettings.Network.NetworkType]())
        //        .AddFederatedPeg(fedPegOptions)
        //        .UseTransactionNotification()
        //        .UseBlockNotification()

        //        .UseMempool()
        //        .AddRPC()
        //        .UsePosConsensus()
        //        .UseWallet()
        //        .AddSQLiteWalletRepository()
        //        .AddPowPosMining()
        //        .UseApi()
        //        .Build();

        //    return node;
        //}

        //private static IFullNode GetSidechainFullNode(string[] args)
        //{
        //    var nodeSettings = new NodeSettings(networksSelector: XoyNetwork.NetworksSelector, protocolVersion: ProtocolVersion.CIRRUS_VERSION, args: args)
        //    {
        //        MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
        //    };

        //    var fedPegOptions = new FederatedPegOptions(
        //        walletSyncFromHeight: new int[] { 1, 1, 1 }[(int)nodeSettings.Network.NetworkType]
        //    );

        //    IFullNode node = new FullNodeBuilder()
        //        .UseNodeSettings(nodeSettings)
        //        .UseBlockStore()
        //        .SetCounterChainNetwork(MainChainNetworks[nodeSettings.Network.NetworkType]())
        //        .UseFederatedPegPoAMining()
        //        .AddFederatedPeg(fedPegOptions)
        //        .CheckForPoAMembersCollateral(true) // This is a mining node so we will check the commitment height data as well as the full set of collateral checks.
        //        .UseTransactionNotification()
        //        .UseBlockNotification()
        //        .UseApi()
        //        .UseMempool()
        //        .AddRPC()
        //        .AddSmartContracts(options =>
        //        {
        //            options.UseReflectionExecutor();
        //            options.UsePoAWhitelistedContracts();
        //        })
        //        .UseSmartContractWallet()
        //        .AddSQLiteWalletRepository()
        //        .Build();

        //    return node;
        //}

        //public static void CreateShortCut()
        //{

        //    string[] argumentList = { "-mainchain", "-sidechain" };

        //    string distinationPath = Directory.GetCurrentDirectory();
        //    Console.WriteLine(distinationPath);
        //    Console.ReadLine();
        //    foreach (var arg in argumentList)
        //    {
        //        object shDesktop = (object)"Desktop";
        //        WshShell shell = new WshShell();
        //        string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\XelsDesktopWalletApp" + arg + ".lnk";
        //        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);

        //        shortcut.Arguments = arg;
        //        shortcut.TargetPath = distinationPath + @"\XelsDesktopWalletApp.exe";
        //        shortcut.Save();
        //    }
        //}


        // xels.xoyminerd start up 


        private const string MainchainArgument = "-mainchain";
        private const string SidechainArgument = "-sidechain";

        private static readonly Dictionary<NetworkType, Func<Network>> MainChainNetworks = new Dictionary<NetworkType, Func<Network>>
        {
            { NetworkType.Mainnet, Networks.Xels.Mainnet },
            { NetworkType.Testnet, Networks.Xels.Testnet },
            { NetworkType.Regtest, Networks.Xels.Regtest }
        };

        [STAThread]
        public static void Main(string[] args)
        {
            args = new string[] { "-mainchain" };
            App app = new App();
            //CreateShortCut();
            MainAsync(args);
            app.InitializeComponent();
            app.Run();

        }

        public static async Task MainAsync(string[] args)
        {
            try
            {
                bool isMainchainNode = args.FirstOrDefault(a => a.ToLower() == MainchainArgument) != null;
                bool isSidechainNode = args.FirstOrDefault(a => a.ToLower() == SidechainArgument) != null;

                if (isSidechainNode == isMainchainNode)
                {
                    throw new ArgumentException($"Gateway node needs to be started specifying either a {SidechainArgument} or a {MainchainArgument} argument");
                }

                IFullNode node = isMainchainNode ? GetXelsNode(args) : GetXoyMiningNode(args);

                if (node != null)
                    await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.Message);
            }
        }

        private static IFullNode GetXoyMiningNode(string[] args)
        {
            var nodeSettings = new NodeSettings(networksSelector: XoyNetwork.NetworksSelector, protocolVersion: ProtocolVersion.CIRRUS_VERSION, args: args)
            {
                MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
            };

            IFullNode node = new FullNodeBuilder()
                .UseNodeSettings(nodeSettings)
                .UseBlockStore()
                .SetCounterChainNetwork(MainChainNetworks[nodeSettings.Network.NetworkType]())
                .UseSmartContractPoAConsensus()
                .UseSmartContractCollateralPoAMining()
                .CheckForPoAMembersCollateral(true) // This is a mining node so we will check the commitment height data as well as the full set of collateral checks.
                .UseTransactionNotification()
                .UseBlockNotification()
                .UseApi()
                .UseMempool()
                .AddRPC()
                .AddSmartContracts(options =>
                {
                    options.UseReflectionExecutor();
                    options.UsePoAWhitelistedContracts();
                })
                .UseSmartContractWallet()
                .AddSQLiteWalletRepository()
                .Build();

            return node;
        }

        /// <summary>
        /// Returns a standard Xels node. Just like XelsD.
        /// </summary>
        private static IFullNode GetXelsNode(string[] args)
        {
            // TODO: Hardcode -addressindex for better user experience

            var nodeSettings = new NodeSettings(networksSelector: Networks.Xels, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: args)
            {
                MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
            };

            IFullNode node = new FullNodeBuilder()
                .UseNodeSettings(nodeSettings)
                .UseBlockStore()
                .UseTransactionNotification()
                .UseBlockNotification()
                .UseApi()
                .UseMempool()
                .AddRPC()
                .UsePosConsensus()
                .UseWallet()
                .AddSQLiteWalletRepository()
                .AddPowPosMining()
                .Build();

            return node;
        }

        public static void CreateShortCut()
        {

            string[] argumentList = { "-mainchain", "-sidechain" };

            string destinationPath = Directory.GetCurrentDirectory();
            //Console.WriteLine(distinationPath);
            //Console.ReadLine();
            foreach (var arg in argumentList)
            {
                object shDesktop = (object)"Desktop";
                WshShell shell = new WshShell();
                string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\xels-app" + arg + ".lnk";
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);

                shortcut.Arguments = arg;
                shortcut.TargetPath = destinationPath + @"\XelsDesktopWalletApp.exe";
                shortcut.Save();
            }
        }


        // miner d end

        //New from Rabbi Vai
        //[STAThread]
        //static void Main(string[] args)
        //{
        //    App application = new App();

        //    try
        //    {
        //        //FreeConsole();
        //        var nodeSettings = new NodeSettings(networksSelector: Networks.Xels,
        //            protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: args)
        //        {
        //            MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
        //        };

        //        IFullNodeBuilder nodeBuilder = new FullNodeBuilder()
        //            .UseNodeSettings(nodeSettings)
        //            .UseBlockStore()
        //            .UseSmartContractWallet()
        //            .UsePosConsensus()
        //            .UseMempool()
        //            .UseColdStakingWallet()
        //            .AddSQLiteWalletRepository()
        //            .AddPowPosMining()
        //            //.UseSmartContractWallet()
        //            .UseApi()
        //            .AddRPC();


        //        if (nodeSettings.EnableSignalR)
        //        {
        //            nodeBuilder.AddSignalR(options =>
        //            {
        //                options.EventsToHandle = new[]
        //                {
        //                    (IClientEvent) new BlockConnectedClientEvent(),
        //                    new TransactionReceivedClientEvent()
        //                };

        //                options.ClientEventBroadcasters = new[]
        //                {
        //                    (Broadcaster: typeof(StakingBroadcaster), ClientEventBroadcasterSettings: new ClientEventBroadcasterSettings
        //                        {
        //                            BroadcastFrequencySeconds = 5
        //                        }),
        //                    (Broadcaster: typeof(WalletInfoBroadcaster), ClientEventBroadcasterSettings: new ClientEventBroadcasterSettings
        //                        {
        //                            BroadcastFrequencySeconds = 5
        //                        })
        //                };
        //            });
        //        }

        //        IFullNode node = nodeBuilder.Build();

        //        if (node != null)
        //            _ = node.RunAsync();

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.ToString());
        //    }

        //    application.InitializeComponent();
        //    application.Run();
        //}

        //End
        //----------------------------------------------------------------------------------------------------------------------


        //Update for smartContract by Noyon

        //[STAThread]
        //public static void Main(string[] args)
        //{
        //    MainAsync(args).Wait();
        //}
        //public static async Task MainAsync(string[] args)
        //{
        //    App application = new App();
        //    try
        //    {
        //        // set the console window title to identify this as a Cirrus full node (for clarity when running Strax and Cirrus on the same machine)
        //        var nodeSettings = new NodeSettings(networksSelector: XoyNetwork.NetworksSelector, protocolVersion: ProtocolVersion.CIRRUS_VERSION, args: args)
        //        {
        //            MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
        //        };

        //        Console.Title = $"Cirrus Full Node {nodeSettings.Network.NetworkType}";

        //        IFullNode node = GetFullNode(nodeSettings);

        //        if (node != null)
        //            await node.RunAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.Message);
        //    }
        //    application.InitializeComponent();
        //    application.Run();
        //}


        //private static IFullNode GetFullNode(NodeSettings nodeSettings)
        //{
        //    // DbType dbType = nodeSettings.GetDbType();

        //    IFullNodeBuilder nodeBuilder = new FullNodeBuilder()
        //    .UseNodeSettings(nodeSettings)
        //    .UseBlockStore()
        //    .UseMempool()
        //    .AddSmartContracts(options =>
        //    {
        //        options.UseReflectionExecutor();
        //        options.UsePoAWhitelistedContracts();
        //    })
        //    //.AddPoAFeature()
        //    //.UsePoAConsensus()
        //    //.CheckCollateralCommitment()

        //    // This needs to be set so that we can check the magic bytes during the Strat to Strax changeover.
        //    // Perhaps we can introduce a block height check rather?

        //    //.SetCounterChainNetwork(StraxNetwork.MainChainNetworks[nodeSettings.Network.NetworkType]())

        //    .UseSmartContractWallet()
        //    .AddSQLiteWalletRepository()
        //    .UseApi()
        //    .AddRPC()
        //    .AddSignalR(options =>
        //    {
        //        options.EventsToHandle = new[]
        //        {
        //            (IClientEvent) new BlockConnectedClientEvent(),
        //           // new FullNodeClientEvent(),
        //           // new ReconstructFederationClientEvent(),
        //            new TransactionReceivedClientEvent(),
        //        };
        //        options.ClientEventBroadcasters = new[]
        //        {
        //            (Broadcaster: typeof(WalletInfoBroadcaster),
        //            ClientEventBroadcasterSettings: new ClientEventBroadcasterSettings
        //            {
        //                BroadcastFrequencySeconds = 5
        //            })
        //        };
        //    })
        //    .UseDiagnosticFeature();

        //    return nodeBuilder.Build();
        //}

        //End
        //----------------------------------------------------------------------------------------------------------------------------
    }
}
