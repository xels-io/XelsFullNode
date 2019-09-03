using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Configuration.Logging;
using Stratis.Patricia;
using Xels.SmartContracts.CLR.Compilation;
using Xels.SmartContracts.CLR.Loader;
using Xels.SmartContracts.CLR.Serialization;
using Xels.SmartContracts.CLR.Validation;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Networks;

namespace Xels.SmartContracts.CLR.Tests
{
    /// <summary>
    /// A mock-less container for all the parts of contract execution.
    /// Most likely just used to get the VM but saves test rewriting for every change inside execution.
    /// </summary>
    public class ContractExecutorTestContext
    {
        public Network Network { get; }
        public IKeyEncodingStrategy KeyEncodingStrategy { get; }
        public ILoggerFactory LoggerFactory { get; }
        public StateRepositoryRoot State { get; }
        public SmartContractValidator Validator { get; }
        public IAddressGenerator AddressGenerator {get;}
        public ContractAssemblyLoader AssemblyLoader { get; }
        public IContractModuleDefinitionReader ModuleDefinitionReader { get; }
        public IContractPrimitiveSerializer ContractPrimitiveSerializer { get; }
        public IInternalExecutorFactory InternalTxExecutorFactory { get; }
        public ReflectionVirtualMachine Vm { get; }
        public ISmartContractStateFactory SmartContractStateFactory { get; }
        public StateProcessor StateProcessor { get; }
        public Serializer Serializer { get; }

        public ContractExecutorTestContext()
        {
            this.Network = new SmartContractsRegTest();
            this.KeyEncodingStrategy = BasicKeyEncodingStrategy.Default;
            this.LoggerFactory = new ExtendedLoggerFactory();
            this.LoggerFactory.AddConsoleWithFilters();
            this.State = new StateRepositoryRoot(new NoDeleteSource<byte[], byte[]>(new MemoryDictionarySource()));
            this.ContractPrimitiveSerializer = new ContractPrimitiveSerializer(this.Network);
            this.Serializer = new Serializer(this.ContractPrimitiveSerializer);
            this.AddressGenerator = new AddressGenerator();
            this.Validator = new SmartContractValidator();
            this.AssemblyLoader = new ContractAssemblyLoader();
            this.ModuleDefinitionReader = new ContractModuleDefinitionReader();
            this.Vm = new ReflectionVirtualMachine(this.Validator, this.LoggerFactory, this.AssemblyLoader, this.ModuleDefinitionReader);
            this.StateProcessor = new StateProcessor(this.Vm, this.AddressGenerator);
            this.InternalTxExecutorFactory = new InternalExecutorFactory(this.LoggerFactory, this.StateProcessor);
            this.SmartContractStateFactory = new SmartContractStateFactory(this.ContractPrimitiveSerializer, this.InternalTxExecutorFactory, this.Serializer);
        }
    }
}
