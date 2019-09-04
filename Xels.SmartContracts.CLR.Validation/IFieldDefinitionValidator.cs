using System.Collections.Generic;
using Mono.Cecil;

namespace Xels.SmartContracts.CLR.Validation
{
    public interface IFieldDefinitionValidator
    {
        IEnumerable<ValidationResult> Validate(FieldDefinition field);
    }
}