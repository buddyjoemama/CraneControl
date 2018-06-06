using CraneWeb.Data;
using SerialLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class ControlController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Operate(List<ControlboardOperation> ops)
        {
            var timer = Stopwatch.StartNew();

            Driver.OperateCrane(ops);
            timer.Stop();

            return Json(timer.ElapsedMilliseconds);
        }

        [HttpPut, Route("api/control/off")]
        public IHttpActionResult Off()
        {
            Driver.Off();
            return Ok();
        }

        [HttpGet, Route("api/control/mag/{on}")]
        public IHttpActionResult OperateMagnet(bool on)
        {
            Driver.ActivateMagnet(on);
            return Ok();
        }
    }
}
