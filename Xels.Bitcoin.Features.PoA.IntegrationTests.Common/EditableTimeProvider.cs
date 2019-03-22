using System;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.PoA.IntegrationTests.Common
{
    public class EditableTimeProvider : DateTimeProvider
    {
        public TimeSpan AdjustedTimeOffset
        {
            get { return this.adjustedTimeOffset; }
            set { this.adjustedTimeOffset = value; }
        }
    }
}
