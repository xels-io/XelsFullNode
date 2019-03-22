using Xels.SmartContracts.Core;
using Xels.SmartContracts.CLR.Compilation;
using Xunit;

public class DemoHelperTests
{
    [Fact]
    public void GetHexStringForDemo()
    {
        ContractCompilationResult compilationResult = ContractCompiler.CompileFile("SmartContracts/Auction.cs");
        string example = compilationResult.Compilation.ToHexString();
    }
}