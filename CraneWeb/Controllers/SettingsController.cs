using Common.Data;
using Newtonsoft.Json;
using SerialLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class SettingsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(String id)
        {
            return Json(Driver.TestPort(id));
        }

        [HttpGet, Route("api/settings/all")]
        public async Task<IHttpActionResult> GetSettings()
        {
            var operations = CraneOperation.GetAll()
                .ToDictionary(k => k.OpCode.ToString(), v => v);

            String portsPath = @"C:\ports.json";
            List<int> availablePorts = new List<int>();
            if (File.Exists(portsPath))
            {
                availablePorts = JsonConvert.DeserializeObject<List<int>>(File.ReadAllText(portsPath));
            }
            else
            {
                availablePorts.Add(8100);
                availablePorts.Add(8101);
            }

            var settings =
                JsonConvert.DeserializeAnonymousType(ConfigurationManager.AppSettings["ServerSettings"],
                new[]
                {
                        new
                        {
                            PortName = "",
                            Value = 0,
                        }
                }).ToDictionary(k => k.PortName, v => v.Value);

            var ip = await Common.NetworkHelper.GetExternalIPAddress();

            return Json(new
            {
                comPort = Driver.FindControllerComPort(),
                operations = operations,
                refreshPort = settings["CameraServerRefreshPort"],
                ipAddress = ip,
                availableCameras = availablePorts
            });
        }
    }
}
