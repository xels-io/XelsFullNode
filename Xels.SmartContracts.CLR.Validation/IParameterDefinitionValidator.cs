using System.Collections.Generic;
using Mono.Cecil;

namespace Xels.SmartContracts.CLR.Validation
{
    public interface IParameterDefinitionValidator
    {
        IEnumerable<ValidationResult> Validate(ParameterDefinition parameter);
    }
}