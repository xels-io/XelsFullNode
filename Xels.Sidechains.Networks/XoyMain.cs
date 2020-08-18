using System;
using System.Collections.Generic;
using System.Net;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.SmartContracts.Networks.Policies;

namespace Xels.Sidechains.Networks
{
    /// <summary>
    /// <see cref="PoANetwork"/>.
    /// </summary>
    public class XoyMain : PoANetwork
    {
        /// <summary> The name of the root folder containing the different federated peg blockchains.</summary>
        private const string NetworkRootFolderName = "xoy";

        /// <summary> The default name used for the federated peg configuration file. </summary>
        private const string NetworkDefaultConfigFilename = "xoy.conf";

        internal XoyMain()
        {
            this.Name = "XoyMain";
            this.NetworkType = NetworkType.Mainnet;
            this.CoinTicker = "XOY";
            this.Magic = 0x522357AC;
            this.DefaultPort = 16179;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.DefaultRPCPort = 16175;
            this.DefaultAPIPort = 37223;
            this.MaxTipAge = 2 * 60 * 60;
            this.MinTxFee = 10000;
            this.FallbackFee = 10000;
            this.MinRelayTxFee = 10000;
            this.RootFolderName = NetworkRootFolderName;
            this.DefaultConfigFilename = NetworkDefaultConfigFilename;
            this.MaxTimeOffsetSeconds = 25 * 60;

            var consensusFactory = new SmartContractCollateralPoAConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1561982325;
            this.GenesisNonce = 3038481;
            this.GenesisBits = new Target(new uint256("00000fffff000000000000000000000000000000000000000000000000000000"));
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            string coinbaseText = "https://github.com/xelsproject/XelsBitcoinFullNode";
            Block genesisBlock = XoyNetwork.CreateGenesis(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward, coinbaseText);

            this.Genesis = genesisBlock;

            // Configure federation public keys used to sign blocks.
            // Keep in mind that order in which keys are added to this list is important
            // and should be the same for all nodes operating on this network.
            var genesisFederationMembers = new List<IFederationMember>()
            {
                new CollateralFederationMember(new PubKey("0237143c9886288a82959f5ec2fd477121ff2447938b2da0cb5af9aaab0e68bf20"), 1_000,"CSJvHV89swdPHRpFJNoLdGQA3pf4criyrZ"), //neo
                new CollateralFederationMember(new PubKey("035d1ff6d7c97d67405e5c21c87ef7d57e4a3e74e6d8033fd6a63e3da5452f05db"), 1_000,"CLnfQobi5T7nokYDrP6J7KCUxCteU5Gopd"), //117
                new CollateralFederationMember(new PubKey("02548edc155625b3c98090906b9b88adc28bf4d22f276e2e5607860d22891e8c19"), 1_000,"CQZqrzamGHFkMJ476GEEYDZ5CsurJzbMsR")  //43.45
                // new FederationMember(new PubKey("0237143c9886288a82959f5ec2fd477121ff2447938b2da0cb5af9aaab0e68bf20")),//neo
                // new FederationMember(new PubKey("035d1ff6d7c97d67405e5c21c87ef7d57e4a3e74e6d8033fd6a63e3da5452f05db")),//117
                // new FederationMember(new PubKey("02548edc155625b3c98090906b9b88adc28bf4d22f276e2e5607860d22891e8c19")) //43.45

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
                autoKickIdleMembers: false
            );

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new NoBIP9Deployments();

            this.Consensus = new Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: consensusOptions,
                coinType: 401,
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
                maxReorgLength: 240, // Heuristic. Roughly 2 * mining members
                defaultAssumeValid: null,
                maxMoney: Money.Coins(100_000_000),
                coinbaseMaturity: 1,
                premineHeight: 2,
                premineReward: Money.Coins(100_000_000),
                proofOfWorkReward: Money.Coins(0),
                powTargetTimespan: TimeSpan.FromDays(14), // two weeks
                powTargetSpacing: TimeSpan.FromMinutes(1),
                powAllowMinDifficultyBlocks: false,
                posNoRetargeting: false,
                powNoRetargeting: true,
                powLimit: null,
                minimumChainWork: null,
                isProofOfStake: false,
                lastPowBlock: 0,
                proofOfStakeLimit: null,
                proofOfStakeLimitV2: null,
                proofOfStakeReward: Money.Zero
            );

            // Same as current smart contracts test networks to keep tests working
            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { 28 }; // C
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { 88 }; // c
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (239) };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x35), (0x87), (0xCF) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x35), (0x83), (0x94) };
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
            this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2b };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 115 };
            this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

            Bech32Encoder encoder = Encoders.Bech32("tb");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            this.Checkpoints = new Dictionary<int, CheckpointInfo>();

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


            this.StandardScriptsRegistry = new SmartContractsStandardScriptsRegistry();

            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("5d4a2bb3b402f7ea66226e99054ec714fd33df8afcd92ec293b36bc38a871488"));
                                                                     
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("70bafcae5db1a6cfa306e05546ec7625373ebb68076fef8bc1d69541f1aa7c4b"));
        }
    }
}