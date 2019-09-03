using FluentAssertions;
using Newtonsoft.Json;
using Xels.Features.FederatedPeg.Models;
using Xels.Features.FederatedPeg.Tests.Utils;
using Xunit;

namespace Xels.Features.FederatedPeg.Tests
{
    public class MaturedBlockRequestModelTests
    {
        [Fact]
        public void ShouldSerialiseAsJson()
        {
            var maturedBlockDeposits = new MaturedBlockRequestModel(TestingValues.GetPositiveInt(), TestingValues.GetPositiveInt());
            string asJson = maturedBlockDeposits.ToString();

            MaturedBlockRequestModel reconverted = JsonConvert.DeserializeObject<MaturedBlockRequestModel>(asJson);

            reconverted.BlockHeight.Should().Be(maturedBlockDeposits.BlockHeight);
            reconverted.MaxBlocksToSend.Should().Be(maturedBlockDeposits.MaxBlocksToSend);
        }
    }
}