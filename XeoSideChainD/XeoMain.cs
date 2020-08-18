using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.PoA.Policies;

namespace XeoSideChainD
{
    /// <summary>
    /// Example network for PoA consensus.
    /// </summary>
    /// <remarks>
    /// Do NOT use this network template exactly as it is when creating your own network.
    /// Redefine federation keys and update genesis block, most importantly timestamp.
    /// Also feel free to change target spacing, premine height and premine reward.
    /// Don't set target spacing to be less than 10 sec.
    /// </remarks>
    public class XeoMain : PoANetwork
    {
        /// <summary> The name of the root folder containing the different PoA blockchains.</summary>
        private const string NetworkRootFolderName = "xeo";

        /// <summary> The default name used for the Xels configuration file. </summary>
        private const string NetworkDefaultConfigFilename = "xeo.conf";

        public PoAConsensusOptions ConsensusOptions => this.Consensus.Options as PoAConsensusOptions;

        public XeoMain()
        {
            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            var messageStart = new byte[4];
            messageStart[0] = 0x76;
            messageStart[1] = 0x36;
            messageStart[2] = 0x23;
            messageStart[3] = 0x06;
            uint magic = BitConverter.ToUInt32(messageStart, 0);

            this.Name = "XeoMain";
            this.NetworkType = NetworkType.Mainnet;
            this.Magic = magic;
            this.DefaultPort = 29787;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.DefaultRPCPort = 29789;
            this.DefaultAPIPort = 37441; // TODO: Confirm
            this.MaxTipAge = 2 * 60 * 60;
            this.MinTxFee = 10000;
            this.FallbackFee = 10000;
            this.MinRelayTxFee = 10000;
            this.RootFolderName = NetworkRootFolderName;
            this.DefaultConfigFilename = NetworkDefaultConfigFilename;
            this.MaxTimeOffsetSeconds = 25 * 60;
            this.CoinTicker = "XEO";

            var consensusFactory = new PoAConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1513622125;
            this.GenesisNonce = 1560058197;
            this.GenesisBits = 402691653;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            Block genesisBlock = CreatePoAGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);

            this.Genesis = genesisBlock;

            // Configure federation.
            // Keep in mind that order in which keys are added to this list is important
            // and should be the same for all nodes operating on this network.
            var genesisFederationMembers = new List<IFederationMember>()
            {
                new FederationMember(new PubKey("03297596f31d8ea9009e8831dcf22b1d66846ee5f0958bcc1b6dfed93782c4e862")),
                new FederationMember(new PubKey("0296d21d25ec915f4a8693ef18c77a3e44176fb565bdb0a4cc747d51d04131e404")),
                new FederationMember(new PubKey("031a8061f5f83d12522c9834324fdb9cd975c255b7e84569304c842a3b61248b1a"))
            };

            var consensusOptions = new PoAConsensusOptions(
                maxBlockBaseSize: 1_000_000,
                maxStandardVersion: 2,
                maxStandardTxWeight: 100_000,
                maxBlockSigopsCost: 20_000,
                maxStandardTxSigopsCost: 20_000 / 5,
                genesisFederationMembers: genesisFederationMembers,
                targetSpacingSeconds: 16,
                votingEnabled: true,
                autoKickIdleMembers: true
            );

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new NoBIP9Deployments();

            this.Consensus = new NBitcoin.Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: consensusOptions,
                coinType: 105,
                hashGenesisBlock: genesisBlock.GetHash(),
                subsidyHalvingInterval: 210000,
                majorityEnforceBlockUpgrade: 750,
                majorityRejectBlockOutdated: 950,
                majorityWindow: 1000,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8"),
                ruleChangeActivationThreshold: 1916, // 95% of 2016
                minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 500,
                defaultAssumeValid: null,
                maxMoney: long.MaxValue,
                coinbaseMaturity: 2,
                premineHeight: 10,
                premineReward: Money.Coins(100_000_000),
                proofOfWorkReward: Money.Coins(0),
                powTargetTimespan: TimeSpan.FromSeconds(14 * 24 * 60 * 60), // two weeks
                powTargetSpacing: TimeSpan.FromSeconds(60),
                powAllowMinDifficultyBlocks: false,
                posNoRetargeting: true,
                powNoRetargeting: true,
                powLimit: null,
                minimumChainWork: null,
                isProofOfStake: false,
                lastPowBlock: 0,
                proofOfStakeLimit: null,
                proofOfStakeLimitV2: null,
                proofOfStakeReward: Money.Zero
            );

            // https://en.bitcoin.it/wiki/List_of_address_prefixes
            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (55) }; // 'P' prefix
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (117) }; // 'p' prefix
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
                { 0, new CheckpointInfo(new uint256("0x0621b88fb7a99c985d695be42e606cb913259bace2babe92970547fa033e4076")) },
            };

            var encoder = new Bech32Encoder("bc");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            this.DNSSeeds = new List<DNSSeedData>();
            this.DNSSeeds = new List<DNSSeedData>
            {
                new DNSSeedData("api.xels.io","api.xels.io")
                //    new DNSSeedData("mainnet2.xelsnetwork.com", "mainnet2.xelsnetwork.com"),
                //    new DNSSeedData("mainnet3.xelsplatform.com", "mainnet3.xelsplatform.com"),
                //    new DNSSeedData("mainnet4.xelsnetwork.com", "mainnet4.xelsnetwork.com")
            };

            this.SeedNodes = new List<NetworkAddress>();
            this.SeedNodes = new List<NetworkAddress>
            {
                new NetworkAddress(IPAddress.Parse("52.68.239.4"), 29787), // Redistribution node
                new NetworkAddress(IPAddress.Parse("54.238.248.117"), 29787), // public node
                new NetworkAddress(IPAddress.Parse("13.114.52.87"), 29787), // public node
                new NetworkAddress(IPAddress.Parse("52.192.229.45"), 29787), // public node
                new NetworkAddress(IPAddress.Parse("52.199.121.139"), 29787 ), // public node

                //new NetworkAddress(IPAddress.Parse("137.116.46.151"), 37221), // public node
                //new NetworkAddress(IPAddress.Parse("40.78.80.159"), 37221), // public node
                //new NetworkAddress(IPAddress.Parse("52.151.86.242"), 37221), // public node
                //new NetworkAddress(IPAddress.Parse("40.74.67.242"), 37221), // public node
            };

            //// No DNS seeds.
            //this.DNSSeeds = new List<DNSSeedData> { };

            //// No seed nodes.
            //string[] seedNodes = { };
            //this.SeedNodes = this.ConvertToNetworkAddresses(seedNodes, this.DefaultPort).ToList();

            this.StandardScriptsRegistry = new PoAStandardScriptsRegistry();

            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x0621b88fb7a99c985d695be42e606cb913259bace2babe92970547fa033e4076"));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("0x9928b372fd9e4cf62a31638607344c03c48731ba06d24576342db9c8591e1432"));

            if ((this.ConsensusOptions.GenesisFederationMembers == null) || (this.ConsensusOptions.GenesisFederationMembers.Count == 0))
            {
                throw new Exception("No keys for initial federation are configured!");
            }
        }

        protected static Block CreatePoAGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string data = "506f41202d204345485450414a6c75334f424148484139205845504839";

            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.Time = nTime;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)42 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(data)))
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
            return genesis;
        }
    }
}
