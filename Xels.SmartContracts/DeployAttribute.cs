using System;

namespace Xels.SmartContracts
{
    /// <summary>
    /// Specifies that this contract should be the one to deploy when there are multiple conttracts inside an assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DeployAttribute : Attribute
    {

    }
}
