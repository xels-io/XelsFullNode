using System;
using System.Collections.Generic;
using System.Net;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xels.Bitcoin.Features.MemoryPool.Rules;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.PoA.BasePoAFeatureConsensusRules;
using Xels.Bitcoin.Features.PoA.Voting.ConsensusRules;
using Xels.Bitcoin.Features.SmartContracts.MempoolRules;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.PoA.MempoolRules;
using Xels.Bitcoin.Features.SmartContracts.PoA.Rules;
using Xels.Bitcoin.Features.SmartContracts.Rules;
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
            this.DefaultPort = 17976;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.DefaultRPCPort = 16175;
            this.DefaultAPIPort = 37223;
            this.DefaultSignalRPort = 38823;
            this.MaxTipAge = 768; // 20% of the fastest time it takes for one MaxReorgLength of blocks to be mined.
            this.MinTxFee = 10000;
            this.FallbackFee = 10000;
            this.MinRelayTxFee = 10000;
            this.RootFolderName = NetworkRootFolderName;
            this.DefaultConfigFilename = NetworkDefaultConfigFilename;
            this.MaxTimeOffsetSeconds = 25 * 60;
            this.DefaultBanTimeSeconds = 1920; // 240 (MaxReorg) * 16 (TargetSpacing) / 2 = 32 Minutes

            var consensusFactory = new SmartContractCollateralPoAConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1614003185;
            this.GenesisNonce = 1290896;
            this.GenesisBits = new Target(new uint256("00000fffff000000000000000000000000000000000000000000000000000000"));
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            string coinbaseText = "There is hope yet! We all need to work together. WE GOT THIS!!!";
            Block genesisBlock = XoyNetwork.CreateGenesis(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward, coinbaseText);

            this.Genesis = genesisBlock;

            // Configure federation public keys used to sign blocks.
            // Keep in mind that order in which keys are added to this list is important
            // and should be the same for all nodes operating on this network.
            var genesisFederationMembers = new List<IFederationMember>()
            {
                new CollateralFederationMember(new PubKey("033762e3baa6628ba1e523e0d3a4b0112f3704467aba0f0fd5788ddf308cd23a43"), new Money(7000000_00000000),"XKQ7qbHnmuCWryhfE7HRnb7cPMdAZtp8QQ"),
                new CollateralFederationMember(new PubKey("02b7b1b8802a5155dbf7ba0fdfca028e995c77da179972a8cf1e99d93462f91d01"), new Money(7000000_00000000),"XXnSQGRrj4D873ySF4m4mucuGQUFGyKpiz"),
                new CollateralFederationMember(new PubKey("02e2a8ee8197604a86131b54e18273f9ccb43f58b9de8c4a611cf27c5e715aea2e"), new Money(7000000_00000000),"XDCJ5fCvjbGY9Tbq9arpeJwCa4Y86EUXoq")
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
            )
            {
                EnforceMinProtocolVersionAtBlockHeight = 1, // setting the value to zero makes the functionality inactive
                EnforcedMinProtocolVersion = NBitcoin.Protocol.ProtocolVersion.CIRRUS_VERSION // minimum protocol version which will be enforced at block height defined in EnforceMinProtocolVersionAtBlockHeight
            };

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

            this.DNSSeeds = new List<DNSSeedData>
            {
                new DNSSeedData("mainnet.xels.io", "mainnet.xels.io")
            };

            this.SeedNodes = new List<NetworkAddress>
            {
                new NetworkAddress(IPAddress.Parse("54.64.43.45"), 17976)
            };

            this.StandardScriptsRegistry = new SmartContractsStandardScriptsRegistry();

            // 16 below should be changed to TargetSpacingSeconds when we move that field.
            Assert(this.DefaultBanTimeSeconds <= this.Consensus.MaxReorgLength * 16 / 2);
            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("000009bf683fdadd315b807e29cd1a26996946d1bf36a09b9f05e55eacfb7c34"));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("64637abf3dd01134a4c7038916396f5cabe26c64c14a41ddd633389a7f4c28c2"));

            this.RegisterRules(this.Consensus);
            this.RegisterMempoolRules(this.Consensus);
        }

        // This should be abstract or virtual
        protected override void RegisterRules(IConsensus consensus)
        {
            // IHeaderValidationConsensusRule -----------------------
            consensus.ConsensusRules
                .Register<HeaderTimeChecksPoARule>()
                .Register<XelsHeaderVersionRule>()
                .Register<PoAHeaderDifficultyRule>()
                .Register<PoAHeaderSignatureRule>();
            // ------------------------------------------------------

            // IIntegrityValidationConsensusRule
            consensus.ConsensusRules
                .Register<BlockMerkleRootRule>()
                .Register<PoAIntegritySignatureRule>();
            // ------------------------------------------------------

            // IPartialValidationConsensusRule
            consensus.ConsensusRules
                .Register<SetActivationDeploymentsPartialValidationRule>()

                // Rules that are inside the method ContextualCheckBlock
                .Register<TransactionLocktimeActivationRule>()
                .Register<CoinbaseHeightActivationRule>()
                .Register<BlockSizeRule>()

                // Rules that are inside the method CheckBlock
                .Register<EnsureCoinbaseRule>()
                .Register<CheckPowTransactionRule>()
                .Register<CheckSigOpsRule>()

                .Register<PoAVotingCoinbaseOutputFormatRule>()
                .Register<AllowedScriptTypeRule>()
                .Register<ContractTransactionPartialValidationRule>();
            // ------------------------------------------------------

            // IFullValidationConsensusRule
            consensus.ConsensusRules
                .Register<SetActivationDeploymentsFullValidationRule>()

                // Rules that require the store to be loaded (coinview)
                .Register<LoadCoinviewRule>()
                .Register<TransactionDuplicationActivationRule>() // implements BIP30

                // Smart contract specific
                .Register<ContractTransactionFullValidationRule>()
                .Register<TxOutSmartContractExecRule>()
                .Register<OpSpendRule>()
                .Register<CanGetSenderRule>()
                .Register<P2PKHNotContractRule>()
                .Register<SmartContractPoACoinviewRule>()
                .Register<SaveCoinviewRule>();
            // ------------------------------------------------------
        }

        protected override void RegisterMempoolRules(IConsensus consensus)
        {
            consensus.MempoolRules = new List<Type>()
            {
                typeof(OpSpendMempoolRule),
                typeof(TxOutSmartContractExecMempoolRule),
                typeof(AllowedScriptTypeMempoolRule),
                typeof(P2PKHNotContractMempoolRule),

                // The non- smart contract mempool rules
                typeof(CheckConflictsMempoolRule),
                typeof(CheckCoinViewMempoolRule),
                typeof(CreateMempoolEntryMempoolRule),
                typeof(CheckSigOpsMempoolRule),
                typeof(CheckFeeMempoolRule),

                // The smart contract mempool needs to do more fee checks than its counterpart, so include extra rules.
                // These rules occur directly after the fee check rule in the non- smart contract mempool.
                typeof(SmartContractFormatLogicMempoolRule),
                typeof(CanGetSenderMempoolRule),
                typeof(AllowedCodeHashLogicMempoolRule), // PoA-specific
                typeof(CheckMinGasLimitSmartContractMempoolRule),

                // Remaining non-SC rules.
                typeof(CheckRateLimitMempoolRule),
                typeof(CheckAncestorsMempoolRule),
                typeof(CheckReplacementMempoolRule),
                typeof(CheckAllInputsMempoolRule)
            };
        }
    }
}
