using CraneWeb.Data;
using Newtonsoft.Json;
using SerialLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public IHttpActionResult GetSettings()
        {
            using (CraneDbContext context = new CraneDbContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                var operations = context.CraneOperations
                    .ToDictionary(k => k.OpCode.ToString(), v => v);

                var settings =
                    JsonConvert.DeserializeAnonymousType(ConfigurationManager.AppSettings["ServerSettings"],
                    new[]
                    {
                        new
                        {
                            PortName = "",
                            Value = 0
                        }
                    }).ToDictionary(k => k.PortName, v => v.Value);

                return Json(new
                {
                    comPort = Driver._com,
                    operations = operations,
                    refreshPort = settings["CameraServerRefreshPort"],
                    ipAddress = Common.NetworkHelper.GetLocalIPAddress()
                });
            }
        }
    }
}
