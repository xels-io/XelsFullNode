using FluentAssertions;
using Newtonsoft.Json;
using Xels.Features.FederatedPeg.Models;
using Xels.Features.FederatedPeg.Tests.Utils;
using Xunit;

namespace Xels.Features.FederatedPeg.Tests
{
    
    public class MaturedBlockDepositModelTests
    {
        [Fact]
        public void ShouldSerialiseAsJson()
        {
            MaturedBlockDepositsModel maturedBlockDeposits = TestingValues.GetMaturedBlockDeposits(3);
            string asJson = maturedBlockDeposits.ToString();

            MaturedBlockDepositsModel reconverted = JsonConvert.DeserializeObject<MaturedBlockDepositsModel>(asJson);

            reconverted.BlockInfo.BlockHash.Should().Be(maturedBlockDeposits.BlockInfo.BlockHash);
            reconverted.BlockInfo.BlockHeight.Should().Be(maturedBlockDeposits.BlockInfo.BlockHeight);
            reconverted.Deposits.Should().BeEquivalentTo(maturedBlockDeposits.Deposits);
        }
    }
}
