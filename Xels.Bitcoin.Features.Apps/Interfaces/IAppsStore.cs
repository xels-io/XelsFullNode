using System.Collections.Generic;

namespace Xels.Bitcoin.Features.Apps.Interfaces
{
    public interface IAppsStore
    {        
        IEnumerable<IXelsApp> Applications { get; }
    }
}
