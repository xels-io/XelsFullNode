﻿using McMaster.Extensions.CommandLineUtils;
using Xels.SmartContracts.Tools.Sct.Build;
using Xels.SmartContracts.Tools.Sct.Deployment;
using Xels.SmartContracts.Tools.Sct.Validation;

namespace Xels.SmartContracts.Tools.Sct
{
    [Command(ThrowOnUnexpectedArgument = false)]
    [Subcommand("validate", typeof(Validator))]
    [Subcommand("build", typeof(Builder))]
    [Subcommand("deploy", typeof(Deployer))]
    [HelpOption]
    [VersionOption("-v|--version", "v0.0.2")]
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();

            return 1;
        }
    }
}