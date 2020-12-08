using System.Collections.Generic;
using Mono.Cecil;

namespace Xels.SmartContracts.CLR.Validation
{
    public interface ITypeDefinitionValidator
    {
        IEnumerable<ValidationResult> Validate(TypeDefinition type);
    }
}