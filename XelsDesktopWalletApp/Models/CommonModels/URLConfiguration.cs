using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models.CommonModels
{
    public class URLConfiguration
    {
        public static string BaseURLMain = "http://localhost:37221/api";  //main chain


        public static string BaseURLSideChain = "http://localhost:37223/api";  //side chain

        public static string BaseURL;//value assagin from program.cs
    }
}
