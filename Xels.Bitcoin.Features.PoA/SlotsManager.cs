﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.PoA
{
    /// <summary>
    /// Provider of information about which pubkey should be used at which timestamp
    /// and what is the next timestamp at which current node will be able to mine.
    /// </summary>
    public class SlotsManager
    {
        private readonly PoAConsensusOptions consensusOptions;

        private readonly FederationManager federationManager;

        private readonly ILogger logger;

        public SlotsManager(Network network, FederationManager federationManager, ILoggerFactory loggerFactory)
        {
            Guard.NotNull(network, nameof(network));
            this.federationManager = Guard.NotNull(federationManager, nameof(federationManager));

            this.consensusOptions = (network as PoANetwork).ConsensusOptions;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>Gets the public key for specified timestamp.</summary>
        /// <param name="headerUnixTimestamp">Timestamp of a header.</param>
        /// <exception cref="ConsensusErrorException">In case timestamp is invalid.</exception>
        public PubKey GetPubKeyForTimestamp(uint headerUnixTimestamp)
        {
            if (!this.IsValidTimestamp(headerUnixTimestamp))
                PoAConsensusErrors.InvalidHeaderTimestamp.Throw();

            List<PubKey> keys = this.consensusOptions.FederationPublicKeys;

            uint roundTime = this.GetRoundLengthSeconds();

            // Time when current round started.
            uint roundStartTimestamp = (headerUnixTimestamp / roundTime) * roundTime;

            // Slot number in current round.
            int currentSlotNumber = (int)((headerUnixTimestamp - roundStartTimestamp) / this.consensusOptions.TargetSpacingSeconds);

            return keys[currentSlotNumber];
        }

        /// <summary>Gets next timestamp at which current node can produce a block.</summary>
        /// <exception cref="Exception">Thrown if this node is not a federation member.</exception>
        public uint GetMiningTimestamp(uint currentTime)
        {
            if (!this.federationManager.IsFederationMember)
                throw new Exception("Not a federation member!");

            // Round length in seconds.
            uint roundTime = this.GetRoundLengthSeconds();

            // Index of a slot that current node can take in each round.
            uint slotIndex = (uint)this.consensusOptions.FederationPublicKeys.IndexOf(this.federationManager.FederationMemberKey.PubKey);

            // Time when current round started.
            uint roundStartTimestamp = (currentTime / roundTime) * roundTime;
            uint nextTimestampForMining = roundStartTimestamp + slotIndex * this.consensusOptions.TargetSpacingSeconds;

            // We already passed our slot in this round.
            // Get timestamp of our slot from next round.
            if (nextTimestampForMining < currentTime)
            {
                // Get timestamp for next round.
                nextTimestampForMining = roundStartTimestamp + roundTime + slotIndex * this.consensusOptions.TargetSpacingSeconds;
            }

            return nextTimestampForMining;
        }

        /// <summary>Determines whether timestamp is valid according to the network rules.</summary>
        public bool IsValidTimestamp(uint headerUnixTimestamp)
        {
            return (headerUnixTimestamp % this.consensusOptions.TargetSpacingSeconds) == 0;
        }

        private uint GetRoundLengthSeconds()
        {
            uint roundLength = (uint)(this.consensusOptions.FederationPublicKeys.Count * this.consensusOptions.TargetSpacingSeconds);

            return roundLength;
        }
    }
}