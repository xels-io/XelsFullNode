using System.Collections.Generic;

namespace Xels.Bitcoin.Features.Apps.Interfaces
{
    public interface IAppsHost
    {
        IEnumerable<IXelsApp> HostedApps { get; }

        void Host(IEnumerable<IXelsApp> xelsApps);        

        void Close();
    }
}
