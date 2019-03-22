using System.Collections.Generic;
using Mono.Cecil;

namespace Xels.SmartContracts.CLR.Validation
{
    public interface IModuleDefinitionValidator
    {
        IEnumerable<ValidationResult> Validate(ModuleDefinition module);
    }
}