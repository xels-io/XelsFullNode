using System.Collections.Generic;
using Mono.Cecil;

namespace Xels.SmartContracts.CLR.Validation
{
    public interface IMemberReferenceValidator
    {
        IEnumerable<ValidationResult> Validate(MemberReference memberReference);
    }
}