using System;
using System.Reflection;

namespace Xels.SmartContracts.CLR.Loader
{
    public interface IContractAssembly
    {
        Assembly Assembly { get; }

        Type GetType(string name);
    }
}
