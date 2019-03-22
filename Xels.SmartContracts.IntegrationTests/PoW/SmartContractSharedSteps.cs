using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;

namespace Xels.SmartContracts.IntegrationTests.PoW
{
    public static class SmartContractSharedSteps
    {
        public static void SendTransaction(CoreNode scSender, CoreNode scReceiver, SmartContractWalletController senderWalletController, string responseHex)
        {
            senderWalletController.SendTransaction(new SendTransactionRequest
            {
                Hex = responseHex
            });

            TestHelper.WaitLoop(() => scReceiver.CreateRPCClient().GetRawMempool().Length > 0);
        }
    }
}