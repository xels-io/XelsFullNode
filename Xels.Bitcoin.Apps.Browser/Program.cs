using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using Xels.Bitcoin.Apps.Browser.Interfaces;
using Xels.Bitcoin.Apps.Browser.Services;

namespace Xels.Bitcoin.Apps.Browser
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
            {
                services.Add(ServiceDescriptor.Singleton<IAppsService, AppsService>());
            });
            
            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }
    }
}
