﻿using System;
using NBitcoin;
using Newtonsoft.Json.Serialization;
using Xels.SmartContracts;

namespace Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor
{
    public class ContractParametersContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly Network network;

        public ContractParametersContractResolver(Network network)
        {
            this.network = network;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            if (objectType == typeof(Address))
            {
                contract.Converter = new AddressJsonConverter(this.network);
            }

            return contract;
        }
    }
}
