using Xels.SmartContracts.CLR.Loader;

namespace Xels.SmartContracts.CLR.Caching
{
    /// <summary>
    /// Holds the items required to execute a contract.
    /// </summary>
    public class CachedAssemblyPackage
    {
        public IContractAssembly Assembly { get; }
        
        public CachedAssemblyPackage(IContractAssembly assembly)
        {
            this.Assembly = assembly;
        }
    }
}
