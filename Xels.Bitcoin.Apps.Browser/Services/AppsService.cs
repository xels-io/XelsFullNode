using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Xels.Bitcoin.Apps.Browser.Dto;
using Xels.Bitcoin.Apps.Browser.Interfaces;

namespace Xels.Bitcoin.Apps.Browser.Services
{
    public class AppsService : IAppsService
    {
        private readonly HttpClient httpClient;

        public AppsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<XelsApp>> GetApplicationsAsync()
        {
            var serializer = new DataContractJsonSerializer(typeof(List<XelsApp>));
            Task<System.IO.Stream> streamTask = this.httpClient.GetStreamAsync("http://localhost:38221/api/apps/all");
            return serializer.ReadObject(await streamTask) as List<XelsApp>;
        }
    }
}
