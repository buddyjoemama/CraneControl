using SerialLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class PortsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(String id)
        {
            return Json(Driver.TestPort(id));
        }

        [HttpGet, Route("api/ports/available")]
        public IHttpActionResult GetConnectedComPort()
        {
            return Json(Driver.FindControllerComPort());
        }
    }
}
