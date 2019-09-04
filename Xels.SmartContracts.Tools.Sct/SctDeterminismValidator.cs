using Xels.SmartContracts.CLR.Validation;
using Xels.SmartContracts.CLR;

namespace Xels.SmartContracts.Tools.Sct
{
    public class SctDeterminismValidator 
    {
        private static readonly ISmartContractValidator Validator = new SmartContractDeterminismValidator(); 
        
        public SmartContractValidationResult Validate(IContractModuleDefinition moduleDefinition)
        {
            return Validator.Validate(moduleDefinition.ModuleDefinition);
        }
    }
}