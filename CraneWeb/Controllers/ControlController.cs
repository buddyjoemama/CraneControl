using Common.Data;
using SerialLib;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class ControlController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Operate(ControlboardOperation op)
        {
            Driver.OperateCrane(op);
            return Ok();
        }

        [HttpPut, Route("api/control/off")]
        public IHttpActionResult Off()
        {
            Driver.Off();
            return Ok();
        }
    }
}
