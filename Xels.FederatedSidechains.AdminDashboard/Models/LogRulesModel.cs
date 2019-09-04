using System;
using System.Collections.Generic;
using System.Linq;
using Xels.FederatedSidechains.AdminDashboard.Entities;
using Xels.FederatedSidechains.AdminDashboard.Helpers;

namespace Xels.FederatedSidechains.AdminDashboard.Models
{
    public class LogRulesModel
    {
        public List<LogRule> Rules { get; set; }

        public LogRulesModel LoadRules(List<LogRule> xelsLogRules, List<LogRule> sidechainLogRules)
        {
            this.Rules = new List<LogRule>();
            foreach (var rule in LogLevelHelper.DefaultLogRules)
            {
                this.Rules.Add(new LogRule()
                {
                    Name = rule,
                    MinLevel = LogLevel.Trace,
                    Filename = string.Empty,
                    XelsActualLevel = LogLevel.Trace,
                    SidechainActualLevel = LogLevel.Trace
                });

                if (xelsLogRules.Any(x => x.Name.Equals(rule)))
                {
                    this.Rules.FirstOrDefault(x => x.Name.Equals(rule)).XelsActualLevel = xelsLogRules.FirstOrDefault(x => x.Name.Equals(rule)).MinLevel;
                }
                else if (sidechainLogRules.Any(x => x.Name.Equals(rule)))
                {
                    this.Rules.FirstOrDefault(x => x.Name.Equals(rule)).SidechainActualLevel = sidechainLogRules.FirstOrDefault(x => x.Name.Equals(rule)).MinLevel;
                }

            }
            return this;
        }
    }
}
