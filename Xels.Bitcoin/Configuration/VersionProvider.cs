﻿using System.Text.RegularExpressions;
using Xels.Bitcoin.Interfaces;

namespace Xels.Bitcoin.Configuration
{
    public class VersionProvider : IVersionProvider
    {
        public string GetVersion()
        {
            Match match = Regex.Match(this.GetType().AssemblyQualifiedName, "Version=([0-9]+\\.[0-9]+\\.[0-9]+)\\.");
            return match.Groups[1].Value;
        }
    }
}