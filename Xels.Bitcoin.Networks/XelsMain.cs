using System;
using System.Collections.Generic;
using System.Net;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xels.Bitcoin.Features.Consensus.Rules.ProvenHeaderRules;
using Xels.Bitcoin.Features.MemoryPool.Rules;
using Xels.Bitcoin.Networks.Deployments;
using Xels.Bitcoin.Networks.Policies;

namespace Xels.Bitcoin.Networks
{
    public class XelsMain : Network
    {
        /// <summary> Xels maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int XelsMaxTimeOffsetSeconds = 25 * 60;

        /// <summary> Xels default value for the maximum tip age in seconds to consider the node in initial block download (2 hours). </summary>
        public const int XelsDefaultMaxTipAgeInSeconds = 2 * 60 * 60;

        /// <summary> The name of the root folder containing the different Xels blockchains (XelsMain, XelsTest, XelsRegTest). </summary>
        public const string XelsRootFolderName = "xels";

        /// <summary> The default name used for the Xels configuration file. </summary>
        public const string XelsDefaultConfigFilename = "xels.conf";

        public XelsMain()
        {
            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            var messageStart = new byte[4];
            messageStart[0] = 0x70;
            messageStart[1] = 0x35;
            messageStart[2] = 0x22;
            messageStart[3] = 0x05;
            uint magic = BitConverter.ToUInt32(messageStart, 0); //0x5223570;; 

            this.Name = "XelsMain";
            this.NetworkType = NetworkType.Mainnet;
            this.Magic = magic;
            this.DefaultPort = 27976;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.DefaultRPCPort = 27975;
            this.DefaultAPIPort = 37221;
            this.DefaultSignalRPort = 38824;
            this.MaxTipAge = 2 * 60 * 60;
            this.MinTxFee = 10000;
            this.FallbackFee = 10000;
            this.MinRelayTxFee = 10000;
            this.RootFolderName = XelsRootFolderName;
            this.DefaultConfigFilename = XelsDefaultConfigFilename;
            this.MaxTimeOffsetSeconds = 25 * 60;
            this.CoinTicker = "XELS";
            this.DefaultBanTimeSeconds = 16000; // 500 (MaxReorg) * 64 (TargetSpacing) / 2 = 4 hours, 26 minutes and 40 seconds

            var consensusFactory = new PosConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = Utils.DateTimeToUnixTime(new DateTimeOffset(2020, 12, 16, 0, 0, 0, TimeSpan.Zero)); //1609459200;
            this.GenesisNonce = 471955; // 0;// 1676861;// 1831645;
            this.GenesisBits = 0x1e0fffff;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            Block genesisBlock = CreateXelsGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);

            this.Genesis = genesisBlock;

            // Taken from XelsX.
            var consensusOptions = new PosConsensusOptions(
                maxBlockBaseSize: 1_000_000,
                maxStandardVersion: 2,
                maxStandardTxWeight: 100_000,
                maxBlockSigopsCost: 20_000,
                maxStandardTxSigopsCost: 20_000 / 5
            );

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new XelsBIP9Deployments()
            {
                [XelsBIP9Deployments.ColdStaking] = new BIP9DeploymentsParameters("ColdStaking", 2,
                    new DateTime(2019, 12, 2, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2020, 12, 2, 0, 0, 0, DateTimeKind.Utc),
                    BIP9DeploymentsParameters.DefaultMainnetThreshold)
            };

            this.Consensus = new NBitcoin.Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: consensusOptions,
                coinType: 105,
                hashGenesisBlock: genesisBlock.GetHash(),
                subsidyHalvingInterval: 768000,
                majorityEnforceBlockUpgrade: 750,
                majorityRejectBlockOutdated: 950,
                majorityWindow: 1000,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8"),
                minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 500,
                defaultAssumeValid: new uint256("0x00000a1f1a078943309f2465bea7807b6f4d288c52b2c2b5f1a7ed13655817d7"), // 1213518
                maxMoney: long.MaxValue,
                coinbaseMaturity: 1,
                premineHeight: 1,
                firstMiningPeriodHeight: 768000,
                secondMiningPeriodHeight: 768000 + 768000,
                thirdMiningPeriodHeight: 768000 + 768000 + 768000,
                forthMiningPeriodHeight: 768000 + 768000 + 768000 + 768000,
                fifthMiningPeriodHeight: 768000 + 768000 + 768000 + 768000 + 768000,
                premineReward: Money.Coins(21000000),
                proofOfWorkReward: Money.Coins(50),
                powTargetTimespan: TimeSpan.FromSeconds(24 * 60 * 60), // two weeks
                powTargetSpacing: TimeSpan.FromSeconds(150),
                powAllowMinDifficultyBlocks: false,
                posNoRetargeting: false,
                powNoRetargeting: false,
                powLimit: new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                minimumChainWork: null,
                isProofOfStake: true,
                lastPowBlock: int.MaxValue - 100,
                proofOfStakeLimit: new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeLimitV2: new BigInteger(uint256.Parse("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeReward: Money.Coins(50)
            );

            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (75) };      // 75 for capital X
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (137) };     // 137 for small x
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (63 + 128) };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
            this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2a };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 23 };
            this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

            this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                { 0, new CheckpointInfo(new uint256("0x00000a1f1a078943309f2465bea7807b6f4d288c52b2c2b5f1a7ed13655817d7"), new uint256("0x0000000000000000000000000000000000000000000000000000000000000000")) }
            };

            this.Bech32Encoders = new Bech32Encoder[2];
            // Bech32 is currently unsupported on Xels - once supported uncomment lines below
            //var encoder = new Bech32Encoder("bc");
            //this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            //this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = null;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = null;

            this.DNSSeeds = new List<DNSSeedData>();
            this.DNSSeeds = new List<DNSSeedData>
            {
                new DNSSeedData("api.xels.io","api.xels.io"),
                new DNSSeedData("mainnet.xels.io","mainnet.xels.io")
                //new DNSSeedData("testnet.xels.io","testnet.xels.io")
                //    new DNSSe                new NetworkAddress(IPAddress.Parse("52.68.239.4"), this.DefaultPort), // Redistribution node with DNS Server EnablededData("mainnet2.xelsnetwork.com", "mainnet2.xelsnetwork.com"),
                //    new DNSSeedData("mainnet3.xelsplatform.com", "mainnet3.xelsplatform.com"),
                //    new DNSSeedData("mainnet4.xelsnetwork.com", "mainnet4.xelsnetwork.com")
            };

            this.SeedNodes = new List<NetworkAddress>();
            this.SeedNodes = new List<NetworkAddress>
            {

                new NetworkAddress(IPAddress.Parse("52.68.239.4"), this.DefaultPort ), // public node with DNS Server Enabled
                new NetworkAddress(IPAddress.Parse("54.64.43.45"), this.DefaultPort ) // public node with DNS Server Enabled
                //new NetworkAddress(IPAddress.Parse("54.238.248.117"), this.DefaultPort), // public node
                //new NetworkAddress(IPAddress.Parse("13.114.52.87"), this.DefaultPort), // public node
                //new NetworkAddress(IPAddress.Parse("52.192.229.45"), this.DefaultPort), // public node
                //new NetworkAddress(IPAddress.Parse("52.199.121.139"), this.DefaultPort ) // public node
                //new NetworkAddress(IPAddress.Parse("54.250.146.43"), this.DefaultPort ) // public node

                //new NetworkAddress(IPAddress.Parse("137.116.46.151"), 37221), // public node
                //new NetworkAddress(IPAddress.Parse("40.78.80.159"), 37221), // public node
                //new NetworkAddress(IPAddress.Parse("52.151.86.242"), 37221), // public node
                //new NetworkAddress(IPAddress.Parse("40.74.67.242"), 37221), // public node
            };

            this.StandardScriptsRegistry = new XelsStandardScriptsRegistry();

            // 64 below should be changed to TargetSpacingSeconds when we move that field.
            Assert(this.DefaultBanTimeSeconds <= this.Consensus.MaxReorgLength * 64 / 2);
            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x00000a1f1a078943309f2465bea7807b6f4d288c52b2c2b5f1a7ed13655817d7"));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("0x4438bf0c97c1a574e3fa3a26f14f36c4d14c9724e5171b400f0474f265c76b2b"));


            this.RegisterRules(this.Consensus);
            this.RegisterMempoolRules(this.Consensus);
        }

        protected void RegisterRules(IConsensus consensus)
        {
            consensus.ConsensusRules
                .Register<HeaderTimeChecksRule>()
                .Register<HeaderTimeChecksPosRule>()
                .Register<XelsBugFixPosFutureDriftRule>()
                .Register<CheckDifficultyPosRule>()
                .Register<XelsHeaderVersionRule>()
                .Register<ProvenHeaderSizeRule>()
                .Register<ProvenHeaderCoinstakeRule>();

            consensus.ConsensusRules
                .Register<BlockMerkleRootRule>()
                .Register<PosBlockSignatureRepresentationRule>()
                .Register<PosBlockSignatureRule>();

            consensus.ConsensusRules
                .Register<SetActivationDeploymentsPartialValidationRule>()
                .Register<PosTimeMaskRule>()

                // rules that are inside the method ContextualCheckBlock
                .Register<TransactionLocktimeActivationRule>()
                .Register<CoinbaseHeightActivationRule>()
                .Register<WitnessCommitmentsRule>()
                .Register<BlockSizeRule>()

                // rules that are inside the method CheckBlock
                .Register<EnsureCoinbaseRule>()
                .Register<CheckPowTransactionRule>()
                .Register<CheckPosTransactionRule>()
                .Register<CheckSigOpsRule>()
                .Register<PosCoinstakeRule>();

            consensus.ConsensusRules
                .Register<SetActivationDeploymentsFullValidationRule>()

                .Register<CheckDifficultyHybridRule>()

                // rules that require the store to be loaded (coinview)
                .Register<LoadCoinviewRule>()
                .Register<TransactionDuplicationActivationRule>()
                .Register<PosCoinviewRule>() // implements BIP68, MaxSigOps and BlockReward calculation
                                             // Place the PosColdStakingRule after the PosCoinviewRule to ensure that all input scripts have been evaluated
                                             // and that the "IsColdCoinStake" flag would have been set by the OP_CHECKCOLDSTAKEVERIFY opcode if applicable.
                .Register<PosColdStakingRule>()
                .Register<SaveCoinviewRule>();
        }

        protected void RegisterMempoolRules(IConsensus consensus)
        {
            consensus.MempoolRules = new List<Type>()
            {
                typeof(CheckConflictsMempoolRule),
                typeof(CheckCoinViewMempoolRule),
                typeof(CreateMempoolEntryMempoolRule),
                typeof(CheckSigOpsMempoolRule),
                typeof(CheckFeeMempoolRule),
                typeof(CheckRateLimitMempoolRule),
                typeof(CheckAncestorsMempoolRule),
                typeof(CheckReplacementMempoolRule),
                typeof(CheckAllInputsMempoolRule),
                typeof(CheckTxOutDustRule)
            };
        }

        protected static Block CreateXelsGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = "There is hope yet! We all need to work together. WE GOT THIS!!!";

            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.Time = nTime;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)42 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
            });

            Block genesis = consensusFactory.CreateBlock();
            genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
            genesis.Header.Bits = nBits;
            genesis.Header.Nonce = nNonce;
            genesis.Header.Version = nVersion;
            genesis.Transactions.Add(txNew);
            genesis.Header.HashPrevBlock = uint256.Zero;
            genesis.UpdateMerkleRoot();

            //var PoWFound = false;
            //while (!PoWFound)
            //{
            //    //if (blockTemplate == null)
            //   // BlockTemplate pblockTemplate = this.blockProvider.BuildPowBlock(this.chain.Tip, new Script()); //blockAssemblerFactory.Create(chainTip).CreateNewBlock(new Script());

            //    Block pblock = genesis;// pblockTemplate.Block;


            //    int nExtraNonce = 0;
            //    ulong maxTries = int.MaxValue;
            //    int InnerLoopCount = int.MaxValue;
            //    //nExtraNonce = this.IncrementExtraNonce(pblock, null, nExtraNonce);

            //    nExtraNonce++;
            //    pblock.UpdateMerkleRoot();

            //    //Block pblock = pblockTemplate.Block;

            //    while ((maxTries > 0) && (pblock.Header.Nonce < InnerLoopCount) && !pblock.CheckProofOfWork())
            //    {
            //        //this.nodeLifetime.ApplicationStopping.ThrowIfCancellationRequested();

            //        ++pblock.Header.Nonce;
            //        --maxTries;
            //    }

            //    if (maxTries == 0)
            //        break;

            //    if (pblock.Header.Nonce == InnerLoopCount)
            //        continue;
            //    //var newChain = new ChainedBlock(pblock.Header, pblock.GetHash(), this.chain.Tip);

            //    //if (newChain.ChainWork <= this.chain.Tip.ChainWork)
            //    //    continue;
            //    PoWFound = true;
            //}


            return genesis;
        }
    }
}
