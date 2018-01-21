using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Common.Configuration;
using System;

namespace CraneAzureFunctions
{
    public static class Forwarder
    {
        [FunctionName("Forwarder")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            String ipaddress = StorageHelper.GetSetting(StorageHelper.StorageKeys.IPAddress)?.Value;
            String port = StorageHelper.GetSetting(StorageHelper.StorageKeys.PublicPort)?.Value;

            var response = req.CreateResponse(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri($"http://{ipaddress}:{port}");

            return response;
        }
    }
}
