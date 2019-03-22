using System;
using Xels.Bitcoin.Utilities;
using Xels.Bitcoin.Utilities.Extensions;
using Xunit;

namespace Xels.Bitcoin.Tests.Utilities
{
    public class DateTimeProviderTest
    {
        [Fact]
        public void GetUtcNowReturnsCurrentUtcDateTime()
        {
            DateTime result = DateTimeProvider.Default.GetUtcNow();

            Assert.Equal(DateTime.UtcNow.ToString("yyyyMMddhhmmss"), result.ToString("yyyyMMddhhmmss"));
        }

        [Fact]
        public void GetTimeOffsetReturnsCurrentUtcTimeOffset()
        {
            DateTimeOffset result = DateTimeProvider.Default.GetTimeOffset();

            Assert.Equal(DateTimeOffset.UtcNow.ToString("yyyyMMddhhmmss"), result.ToString("yyyyMMddhhmmss"));
        }

        [Fact]
        public void GetTimeReturnsUnixTimeStamp()
        {
            long timeStamp = DateTimeProvider.Default.GetTime();

            Assert.Equal(DateTime.UtcNow.ToUnixTimestamp(), timeStamp);
        }
    }
}
