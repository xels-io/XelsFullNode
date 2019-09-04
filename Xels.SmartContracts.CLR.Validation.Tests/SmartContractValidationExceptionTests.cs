using System.Collections.Generic;
using Mono.Cecil;
using Xels.SmartContracts.CLR.Exceptions;
using Xels.SmartContracts.CLR.Validation.Validators;
using Xels.SmartContracts.CLR.Validation.Validators.Type;
using Xunit;

namespace Xels.SmartContracts.CLR.Validation.Tests
{
    public class SmartContractValidationExceptionTests
    {

        [Fact]
        public void ToStringOutputIsUseful()
        {
            SmartContractValidationException exception = new SmartContractValidationException(new List<ValidationResult>
            {
                new WhitelistValidator.DeniedMemberValidationResult("Subject 1", "Validation Type 1", "Message 1"),
                new FinalizerValidator.FinalizerValidationResult(new TypeDefinition("Namespace", "Name", TypeAttributes.Abstract))
            });

            string output = exception.ToString();
            Assert.Contains("Type Name defines a finalizer", output);
            Assert.Contains("Message 1", output);
        }
    }
}
