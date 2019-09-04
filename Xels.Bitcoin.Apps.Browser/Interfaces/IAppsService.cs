using System.Collections.Generic;
using System.Threading.Tasks;
using Xels.Bitcoin.Apps.Browser.Dto;

namespace Xels.Bitcoin.Apps.Browser.Interfaces
{
    /// <summary>
    /// Service abstraction that communicates with the AppsController to return XelsApp data.
    /// Implementation is injected into components as required.
    /// </summary>
    public interface IAppsService
    {
        Task<List<XelsApp>> GetApplicationsAsync();
    }
}
