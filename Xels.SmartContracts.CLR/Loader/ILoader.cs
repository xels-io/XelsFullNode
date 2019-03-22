using CSharpFunctionalExtensions;

namespace Xels.SmartContracts.CLR.Loader
{
    public interface ILoader
    {
        Result<IContractAssembly> Load(ContractByteCode bytes);
    }
}
