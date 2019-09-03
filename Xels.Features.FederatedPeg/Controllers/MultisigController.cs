using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.Utilities;
using Xels.Bitcoin.Utilities.JsonErrors;
using Xels.Bitcoin.Utilities.ModelStateErrors;
using Xels.Features.FederatedPeg.Models;

namespace Xels.Features.FederatedPeg.Controllers
{
    /// <summary>
    /// Controller providing operations on a wallet.
    /// </summary>
    [Route("api/[controller]")]
    public class MultisigController : Controller
    {
        private readonly FedMultiSigManualWithdrawalTransactionBuilder fedMultiSigManualWithdrawalTransactionBuilder;

        /// <summary>Specification of the network the node runs on - regtest/testnet/mainnet.</summary>
        private readonly Network network;

        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        public MultisigController(
            ILoggerFactory loggerFactory,
            FedMultiSigManualWithdrawalTransactionBuilder fedMultiSigManualWithdrawalTransactionBuilder,
            Network network)
        {
            this.fedMultiSigManualWithdrawalTransactionBuilder = fedMultiSigManualWithdrawalTransactionBuilder;
            this.network = network;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Builds a transaction and returns the hex to use when executing the transaction.
        /// </summary>
        /// <param name="request">An object containing the parameters used to build a transaction.</param>
        /// <returns>A JSON object including the transaction ID, the hex used to execute
        /// the transaction, and the transaction fee.</returns>
        [Route("build-transaction")]
        [HttpPost]
        public IActionResult BuildTransaction([FromBody] BuildMultisigTransactionRequest request)
        {
            Guard.NotNull(request, nameof(request));

            if (!this.ModelState.IsValid)
                return ModelStateErrors.BuildErrorResponse(this.ModelState);

            try
            {
                var recipients = request
                    .Recipients
                    .Select(recipientModel => new Wallet.Recipient
                    {
                        ScriptPubKey = BitcoinAddress.Create(recipientModel.DestinationAddress, this.network).ScriptPubKey,
                        Amount = recipientModel.Amount
                    })
                    .ToList();

                Key[] privateKeys = request
                    .Secrets
                    .Select(secret => new Mnemonic(secret.Mnemonic).DeriveExtKey(secret.Passphrase).PrivateKey)
                    .ToArray();

                Transaction transactionResult = this.fedMultiSigManualWithdrawalTransactionBuilder.BuildTransaction(recipients, privateKeys);

                var model = new WalletBuildTransactionModel
                {
                    Hex = transactionResult.ToHex(),
                    TransactionId = transactionResult.GetHash()
                };

                return this.Json(model);
            }
            catch (Exception e)
            {
                LoggerExtensions.LogError(this.logger, "Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
    }
}