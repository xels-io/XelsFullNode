using FluentAssertions;
using Newtonsoft.Json;
using Xels.Features.FederatedPeg.Interfaces;
using Xels.Features.FederatedPeg.TargetChain;
using Xels.Features.FederatedPeg.Tests.Utils;
using Xunit;

namespace Xels.Features.FederatedPeg.Tests
{
    public class WithdrawalTests
    {
        [Fact]
        public void ShouldSerialiseAsJson()
        {
            IWithdrawal withdrawal = TestingValues.GetWithdrawal();

            string asJson = withdrawal.ToString();
            Withdrawal reconverted = JsonConvert.DeserializeObject<Withdrawal>(asJson);

            reconverted.BlockHash.Should().Be(withdrawal.BlockHash);
            reconverted.Amount.Satoshi.Should().Be(withdrawal.Amount.Satoshi);
            reconverted.BlockNumber.Should().Be(withdrawal.BlockNumber);
            reconverted.Id.Should().Be(withdrawal.Id);
            reconverted.DepositId.Should().Be(withdrawal.DepositId);
            reconverted.TargetAddress.Should().Be(withdrawal.TargetAddress);
        }
    }
}
