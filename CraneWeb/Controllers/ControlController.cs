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
            try
            {
                Driver.OperateCrane(op);
            }
            catch { }
            return Ok();
        }

        [HttpPut, Route("api/control/off")]
        public IHttpActionResult Off()
        {
            try
            {
                Driver.Off();
            }
            catch
            {

            }

            return Ok();
        }
    }
}
