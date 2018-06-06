using CraneWeb.Data;
using SerialLib;
using System;
using System.Collections.Generic;
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

                return Json(new
                {
                    comPort = Driver._com,
                    operations = operations
                });
            }
        }
    }
}
