using System.Collections.Generic;
using Xels.SmartContracts.Tools.Sct.Report.Elements;

namespace Xels.SmartContracts.Tools.Sct.Report.Sections
{
    /// <summary>
    /// Represents the section of a smart contract validation report
    /// that outputs the bytecode of a compiled contract.
    /// </summary>
    public class ByteCodeSection : IReportSection
    {
        public IEnumerable<IReportElement> CreateSection(ValidationReportData data)
        {
            if (data.CompilationSuccess && data.FormatValid && data.DeterminismValid)
            {
                yield return new ReportElement("ByteCode");
                yield return new ReportElement(data.CompilationBytes.ToHexString());
            }
        }
    }
}