using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Xels.Bitcoin.Features.Apps.Interfaces;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.Apps
{
    /// <summary>
    /// Reponsible for web-hosting XelsApps
    /// </summary>
    public class AppsHost : IDisposable, IAppsHost
    {        
        private readonly ILogger logger;
        private readonly List<(IXelsApp app, IWebHost host)> hostedApps;

        public AppsHost(ILoggerFactory loggerFactory) 
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
            this.hostedApps = new List<(IXelsApp app, IWebHost host)>();
        }

        public IEnumerable<IXelsApp> HostedApps => this.hostedApps.Select(x => x.app);

        public void Host(IEnumerable<IXelsApp> xelsApps)
        {
            xelsApps.Where(x => x.IsSinglePageApp).ToList().ForEach(this.HostSinglePageApp);
        }

        public void Close() => this.Dispose();

        public void Dispose()
        {
            this.hostedApps.ForEach(x => x.host.Dispose());
            this.hostedApps.Clear();
        }

        private void HostSinglePageApp(IXelsApp xelsApp)
        {
            try
            {
                int[] nextFreePort = { 0 };
                IpHelper.FindPorts(nextFreePort);
                xelsApp.Address = $"http://localhost:{nextFreePort.First()}";

                (IXelsApp app, IWebHost host) pair = (xelsApp, new WebHostBuilder()
                            .UseKestrel()
                            .UseIISIntegration()
                            .UseWebRoot(Path.Combine(xelsApp.Location, xelsApp.WebRoot))
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseUrls(xelsApp.Address)
                            .UseStartup<SinglePageStartup>()
                            .Build());

                pair.host.Start();                

                this.hostedApps.Add(pair);                
                this.logger.LogInformation("SPA '{0}' hosted at {1}", xelsApp.DisplayName, xelsApp.Address);
            }
            catch (Exception e)
            {
                this.logger.LogError("Failed to host app '{0}' :{1}", xelsApp.DisplayName, e.Message);
            }
        }
    }
}
