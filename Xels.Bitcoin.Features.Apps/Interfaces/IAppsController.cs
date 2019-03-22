using Microsoft.AspNetCore.Mvc;

namespace Xels.Bitcoin.Features.Apps.Interfaces
{
    public interface IAppsController
    {
        IActionResult GetApplications();
    }
}
