using SerialLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class ControlController : ApiController
    {
        public IHttpActionResult Get(CraneActions action)
        {
            SerialLib.Driver.Control(action);
            return Ok(new { Action = Enum.GetName(typeof(CraneActions), action) });
        }

        [HttpGet, Route("api/control/ports")]
        public IHttpActionResult GetAvailablePorts()
        {
            return Json(SerialLib.Driver.EnumeratePorts());
        }
    }
}
