using System.Collections.Generic;
using Xels.SmartContracts.Tools.Sct.Report.Elements;

namespace Xels.SmartContracts.Tools.Sct.Report.Sections
{
    /// <summary>
    /// Represents the section of a smart contract validation report
    /// that outputs compilation results.
    /// </summary>
    public class CompilationSection : IReportSection
    {
        public IEnumerable<IReportElement> CreateSection(ValidationReportData data)
        {
            yield return new ReportElement($"Compilation Result");
            yield return new ReportElement($"Compilation OK: {data.CompilationSuccess}");

            foreach (var compilationError in data.CompilationErrors)
            {
                yield return new ReportElement($"Error: {compilationError.Message}");
            }

            yield return new NewLineElement();
        }
    }
}