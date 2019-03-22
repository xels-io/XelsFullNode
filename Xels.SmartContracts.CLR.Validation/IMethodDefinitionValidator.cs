using System.Collections.Generic;
using Mono.Cecil;

namespace Xels.SmartContracts.CLR.Validation
{
    public interface IMethodDefinitionValidator
    {
        IEnumerable<ValidationResult> Validate(MethodDefinition method);
    }
}