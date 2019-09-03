using System;
using System.Collections.Generic;
using System.Net;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.Protocol;
using Xels.Bitcoin.Networks.Deployments;
using Xels.Bitcoin.Networks.Policies;

namespace Xels.Bitcoin.Networks
{
    public class XelsTest : XelsMain
    {
        public XelsTest()
        {
            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            var messageStart = new byte[4];
            messageStart[0] = 0x71;
            messageStart[1] = 0x31;
            messageStart[2] = 0x21;
            messageStart[3] = 0x11;
            uint magic = BitConverter.ToUInt32(messageStart, 0); // 0x11213171;

            this.Name = "XelsTest";
            this.NetworkType = NetworkType.Testnet;
            this.Magic = magic;
            this.DefaultPort = 38221;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.DefaultRPCPort = 26174;
            this.DefaultAPIPort = 38221;
            this.CoinTicker = "TXELS";

            var powLimit = new Target(new uint256("0000ffff00000000000000000000000000000000000000000000000000000000"));

            var consensusFactory = new PosConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1470467000;
            this.GenesisNonce = 1831645;
            this.GenesisBits = 0x1e0fffff;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            Block genesisBlock = CreateXelsGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);

            genesisBlock.Header.Time = 1493909211;
            genesisBlock.Header.Nonce = 2433759;
            genesisBlock.Header.Bits = powLimit;

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
                [XelsBIP9Deployments.ColdStaking] = new BIP9DeploymentsParameters(2,
                    new DateTime(2018, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2019, 6, 1, 0, 0, 0, DateTimeKind.Utc))
            };

            this.Consensus = new NBitcoin.Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: consensusOptions,
                coinType: 105,
                hashGenesisBlock: genesisBlock.GetHash(),
                subsidyHalvingInterval: 1014286,
                majorityEnforceBlockUpgrade: 750,
                majorityRejectBlockOutdated: 950,
                majorityWindow: 1000,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8"),
                ruleChangeActivationThreshold: 1916, // 95% of 2016
                minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 500,
                //defaultAssumeValid: new uint256("0xc9a15c9dd87c6219b273f93442b87fdaf9eebb4f3059d8ed8239c41a4ab3e730"), // 780785
                defaultAssumeValid: new uint256("0x00000e246d7b73b88c9ab55f2e5e94d9e22d471def3df5ea448f5576b1d156b9"), // 372652
                maxMoney: 2537175000 * Money.COIN,  //long.MaxValue,
                coinbaseMaturity: 1, //10,
                premineHeight: 13, //2,
                firstMiningPeriodHeight: 850000,
                secondMiningPeriodHeight: 850000 + 500000,
                thirdMiningPeriodHeight: 850000 + 500000 + 850000,
                forthMiningPeriodHeight: 850000 + 500000 + 850000 + 500000,
                premineReward: Money.Coins(187155000),  //Money.Coins(98000000),
                proofOfWorkReward: Money.Coins(375), //(4),
                powTargetTimespan: TimeSpan.FromSeconds(24 * 60 * 60), //(14 * 24 * 60 * 60), // two weeks
                powTargetSpacing: TimeSpan.FromSeconds(150), //(10 * 60),
                powAllowMinDifficultyBlocks: false,
                posNoRetargeting: false,
                powNoRetargeting: false,
                powLimit: powLimit,
                minimumChainWork: null,
                isProofOfStake: true,
                lastPowBlock: 3, //12500,
                proofOfStakeLimit: new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeLimitV2: new BigInteger(uint256.Parse("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeReward: Money.Coins(375) //.COIN
            );

            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (75) };      // 75 for capital X    { (65) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (137) };     // 137 for small x    { (196) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (65 + 128) };

            this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                //{ 0, new CheckpointInfo(new uint256("0x0000033ab02dbdf95788721b78fcbaac559fde8bdc0948ad2dbeeedf45c99c4e"), new uint256("0x0000000000000000000000000000000000000000000000000000000000000000")) }
                { 0, new CheckpointInfo(new uint256("0x00000e246d7b73b88c9ab55f2e5e94d9e22d471def3df5ea448f5576b1d156b9"), new uint256("0x0000000000000000000000000000000000000000000000000000000000000000")) }
            };

            this.DNSSeeds = new List<DNSSeedData>();
            //this.DNSSeeds = new List<DNSSeedData>
            //{
            //    new DNSSeedData("testnet1.xelsplatform.com", "testnet1.xelsplatform.com"),
            //    new DNSSeedData("testnet2.xelsplatform.com", "testnet2.xelsplatform.com"),
            //    new DNSSeedData("testnet3.xelsplatform.com", "testnet3.xelsplatform.com"),
            //    new DNSSeedData("testnet4.xelsplatform.com", "testnet4.xelsplatform.com")
            //};

            this.SeedNodes = new List<NetworkAddress>();
            //this.SeedNodes = new List<NetworkAddress>
            //{
            //    new NetworkAddress(IPAddress.Parse("51.140.231.125"), 26178), // danger cloud node
            //    new NetworkAddress(IPAddress.Parse("13.70.81.5"), 26178), // beard cloud node
            //    new NetworkAddress(IPAddress.Parse("191.235.85.131"), 26178), // fassa cloud node
            //};

            this.StandardScriptsRegistry = new XelsStandardScriptsRegistry();

            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x00000e246d7b73b88c9ab55f2e5e94d9e22d471def3df5ea448f5576b1d156b9"));
        }
    }
}
