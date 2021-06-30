﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models.SmartContractModels
{
    public class TokenModel
    {

        public string Ticker { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Decimals { get; set; }

        public string Balance { get; set; }
        public string DropDownValue { get; set; }
    }

    public class TokenRetrieveModel
    {

        public string Ticker { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Decimals { get; set; }

        public string Balance { get; set; }
    }
}
