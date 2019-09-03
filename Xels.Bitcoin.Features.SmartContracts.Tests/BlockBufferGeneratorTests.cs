using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Miner;
using Xels.SmartContracts.Networks;
using Xunit;

namespace Xels.Bitcoin.Features.SmartContracts.Tests
{
    public class BlockBufferGeneratorTests
    {
        private readonly BlockBufferGenerator bufferGenerator;

        public BlockBufferGeneratorTests()
        {
            this.bufferGenerator = new BlockBufferGenerator();
        }

        [Fact]
        public void Buffer_50Kb_For_1MB_BlockSize()
        {
            BlockDefinitionOptions optionsFromNetwork = new MinerSettings(new NodeSettings(new SmartContractsRegTest())).BlockDefinitionOptions;
            BlockDefinitionOptions newOptions = this.bufferGenerator.GetOptionsWithBuffer(optionsFromNetwork);

            Assert.Equal((uint)950_000, newOptions.BlockMaxWeight);
            Assert.Equal((uint)950_000, newOptions.BlockMaxSize);
        }
    }
}
