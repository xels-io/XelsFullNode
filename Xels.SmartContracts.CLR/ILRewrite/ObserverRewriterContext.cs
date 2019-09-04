using Mono.Cecil.Cil;

namespace Xels.SmartContracts.CLR.ILRewrite
{
    public class ObserverRewriterContext
    {
        public ObserverReferences Observer { get; }

        public VariableDefinition ObserverVariable { get; }

        public ObserverRewriterContext(ObserverReferences observer, VariableDefinition observerVariable)
        {
            this.Observer = observer;
            this.ObserverVariable = observerVariable;
        }
    }
}
