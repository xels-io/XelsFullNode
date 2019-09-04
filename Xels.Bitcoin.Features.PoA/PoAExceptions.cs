using System;

namespace Xels.Bitcoin.Features.PoA
{
    public class NotAFederationMemberException : Exception
    {
        public NotAFederationMemberException() : base("Not a federation member!")
        {
        }
    }
}
