using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Apps.Interfaces;
using Xels.Bitcoin.Utilities;
using Xels.Bitcoin.Utilities.Extensions;

namespace Xels.Bitcoin.Features.Apps
{
    /// <summary>
    /// Responsible for storing XelsApps as read from the current running Xels folder.
    /// </summary>
    public class AppsStore : IAppsStore
    {
        private readonly ILogger logger;
        private List<IXelsApp> applications;
        private readonly DataFolder dataFolder;
        private const string ConfigFileName = "xelsApp.json";

        public AppsStore(ILoggerFactory loggerFactory, DataFolder dataFolder)
        {
            this.dataFolder = dataFolder;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        public IEnumerable<IXelsApp> Applications
        {
            get
            {
                this.Load();

                return this.applications;
            }
        }

        private void Load()
        {
            try
            {
                if (this.applications != null)
                    return;

                this.applications = new List<IXelsApp>();

                FileInfo[] fileInfos = new DirectoryInfo(this.dataFolder.ApplicationsPath)
                    .GetFiles(ConfigFileName, SearchOption.AllDirectories);

                IEnumerable<IXelsApp> apps = fileInfos.Select(x => new FileStorage<XelsApp>(x.DirectoryName))
                                                         .Select(this.CreateAppInstance);

                this.applications.AddRange(apps.Where(x => x != null));

                if (this.applications.IsEmpty())
                    this.logger.LogWarning("No Xels applications found at or below '{0}'.", this.dataFolder.ApplicationsPath);
            }
            catch (Exception e)
            {
                this.logger.LogError("Failed to load Xels apps :{0}", e.Message);
            }
        }

        private IXelsApp CreateAppInstance(FileStorage<XelsApp> fileStorage)
        {
            try
            {
                XelsApp xelsApp = fileStorage.LoadByFileName(ConfigFileName);
                xelsApp.Location = fileStorage.FolderPath;
                return xelsApp;
            }
            catch (Exception e)
            {
                this.logger.LogError("Failed to create app :{0}", e.Message);
                return null;
            }
        }
    }
}
