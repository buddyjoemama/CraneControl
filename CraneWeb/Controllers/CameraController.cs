using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class CameraController : ApiController
    {
        [HttpGet, Route("api/camera/{id}")]
        public HttpResponseMessage GetImage(int id)
        {
            return null;
            //Image img = (Image)data.SingleOrDefault();
            //byte[] imgData = img.ImageData;
            //MemoryStream ms = new MemoryStream(imgData);
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            //response.Content = new StreamContent(ms);
            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            //return response;
        }
    }
}
