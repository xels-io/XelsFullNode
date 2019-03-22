using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Xels.SmartContracts.CLR.Validation
{
    public interface IInstructionValidator
    {
        IEnumerable<ValidationResult> Validate(Instruction instruction, MethodDefinition method);
    }
}