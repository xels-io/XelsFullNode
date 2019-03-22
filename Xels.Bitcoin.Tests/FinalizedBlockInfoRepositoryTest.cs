using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Utilities;
using Xunit;

namespace Xels.Bitcoin.Tests
{
    public class FinalizedBlockInfoRepositoryTest : TestBase
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly DBreezeSerializer dBreezeSerializer;

        public FinalizedBlockInfoRepositoryTest() : base(KnownNetworks.XelsRegTest)
        {
            this.loggerFactory = new LoggerFactory();
            this.dBreezeSerializer = new DBreezeSerializer(this.Network);
        }

        [Fact]
        public async Task FinalizedHeightSavedOnDiskAsync()
        {
            string dir = CreateTestDir(this);

            using (var repo = new FinalizedBlockInfoRepository(dir, this.loggerFactory, this.dBreezeSerializer))
            {
                repo.SaveFinalizedBlockHashAndHeight(uint256.One, 777);
            }

            using (var repo = new FinalizedBlockInfoRepository(dir, this.loggerFactory, this.dBreezeSerializer))
            {
                await repo.LoadFinalizedBlockInfoAsync(this.Network);
                Assert.Equal(777, repo.GetFinalizedBlockInfo().Height);
            }
        }

        [Fact]
        public async Task FinalizedHeightCantBeDecreasedAsync()
        {
            string dir = CreateTestDir(this);

            using (var repo = new FinalizedBlockInfoRepository(dir, this.loggerFactory, this.dBreezeSerializer))
            {
                repo.SaveFinalizedBlockHashAndHeight(uint256.One, 777);
                repo.SaveFinalizedBlockHashAndHeight(uint256.One, 555);

                Assert.Equal(777, repo.GetFinalizedBlockInfo().Height);
            }

            using (var repo = new FinalizedBlockInfoRepository(dir, this.loggerFactory, this.dBreezeSerializer))
            {
                await repo.LoadFinalizedBlockInfoAsync(this.Network);
                Assert.Equal(777, repo.GetFinalizedBlockInfo().Height);
            }
        }
    }
}
