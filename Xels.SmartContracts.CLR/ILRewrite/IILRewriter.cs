using Mono.Cecil;

namespace Xels.SmartContracts.CLR.ILRewrite
{
    public interface IILRewriter
    {
        /// <summary>
        /// Rewrites the IL of the given module and returns it.
        /// </summary>
        ModuleDefinition Rewrite(ModuleDefinition module);
    }
}
