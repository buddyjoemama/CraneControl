using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class CameraController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get(String port)
        {
            HttpClient client = new HttpClient();
            var id = (new Random()).NextDouble();
            var rand = (new Random()).NextDouble();

            String ip = null;
            try
            {
                ip = await Common.NetworkHelper.GetExternalIPAddress();
            }
            catch { }

            var result = await client.GetAsync($"http://{ip}:{port}/get?id={id}&r={rand}");

            return Json(new
            {
                id,
                status = result.StatusCode
            });
        }
    }
}
