using System.Collections.Generic;

namespace Xels.SmartContracts.Tools.Sct.Report
{
    public interface IReportRenderer
    {
        void Render(IEnumerable<IReportSection> sections, ValidationReportData data);
    }
}