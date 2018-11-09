using SerialLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class PVTController : ApiController
    {
        [HttpPost]
        public IHttpActionResult OperatePVT(PvtAction action)
        {
            try
            {
                Driver.OperatePVT(action.action);
            }
            catch { }
            return Ok();
        }

        public class PvtAction
        {
            public PvtActions action { get; set; }
        }
    }
}
