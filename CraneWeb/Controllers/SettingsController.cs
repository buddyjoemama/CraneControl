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


            String ip = null;
            try
            {
                ip = await Common.NetworkHelper.GetExternalIPAddress();
            }
            catch { }

            return Json(new
            {
                operations,
                ipAddress = ip,
                pvtActions = Enum.GetNames(typeof(PvtActions))
                    .Select(s=> new
                    {
                        name = s,
                        id = Enum.Parse(typeof(PvtActions), s)
                    }),
                cameras = new []
                {
                    new
                    {
                        supportsPvt = true,
                        port = 8100
                    },
                    new
                    {
                        supportsPvt = false,
                        port = 8181
                    }
                }
            });
        }
    }
}
