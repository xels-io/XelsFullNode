using System;
using NBitcoin;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Utilities;
using Xels.Bitcoin.Utilities.Extensions;

namespace Xels.SmartContracts.Tests.Common.MockChain
{
    public class TargetSpacingDateTimeProvider : IDateTimeProvider
    {
        private DateTime backing;
        private readonly uint spacing;

        public TargetSpacingDateTimeProvider(Network network)
        {
            this.backing = DateTime.Now;
            this.spacing = (network.Consensus.Options as PoAConsensusOptions).TargetSpacingSeconds;
        }

        public long GetTime()
        {
            return this.backing.ToUnixTimestamp();
        }

        public DateTime GetUtcNow()
        {
            return this.backing;
        }

        public DateTime GetAdjustedTime()
        {
            return this.backing;
        }

        public DateTimeOffset GetTimeOffset()
        {
            return this.backing;
        }

        public long GetAdjustedTimeAsUnixTimestamp()
        {
            return this.backing.ToUnixTimestamp();
        }

        public void SetAdjustedTimeOffset(TimeSpan adjusted)
        {
            throw new NotImplementedException();
        }

        public void NextSpacing()
        {
            this.backing = this.backing.AddSeconds(this.spacing);
        }
    }
}
