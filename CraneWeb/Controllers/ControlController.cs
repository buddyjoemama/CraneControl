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
        [HttpGet, Route("api/control/operate/{northChip}/{southChip}/{mag}")]
        public IHttpActionResult Operate(NorthChipActions northChip, SouthChipActions southChip, bool mag = false)
        {
            SerialLib.Driver.Control(CraneActions.On, northChip, southChip, mag);

            return Ok(new
            {
                NorthChip = Enum.GetName(typeof(NorthChipActions), northChip),
                SouthChip = Enum.GetName(typeof(SouthChipActions), southChip),
                MagOn = mag
            });
        }

        [HttpPut, Route("api/control/off")]
        public IHttpActionResult Off()
        {
            SerialLib.Driver.Activate(CraneActions.Off);
            return Ok();
        }

        [HttpGet, Route("api/control/ports")]
        public IHttpActionResult GetAvailablePorts()
        {
            return Json(SerialLib.Driver.EnumeratePorts());
        }
    }
}
