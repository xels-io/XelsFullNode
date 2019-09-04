using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Xels.SmartContracts.CLR.Validation
{
    public sealed class SmartContractValidator : ISmartContractValidator
    {
        public SmartContractValidationResult Validate(ModuleDefinition moduleDefinition)
        {
            ValidationPolicy policy = ValidationPolicy.FromExisting(new[] { FormatPolicy.Default, DeterminismPolicy.Default });
            var validator = new ModulePolicyValidator(policy);

            List<ValidationResult> results = validator.Validate(moduleDefinition).ToList();
            return new SmartContractValidationResult(results);
        }
    }
}