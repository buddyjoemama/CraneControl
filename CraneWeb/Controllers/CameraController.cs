﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CraneWeb.Controllers
{
    public class CameraController : ApiController
    {
        //[HttpGet, Route("api/camera/{id}")]
        //public HttpResponseMessage GetImage(int id)
        //{
        //    return null;
        //    //Image img = (Image)data.SingleOrDefault();
        //    //byte[] imgData = img.ImageData;
        //    //MemoryStream ms = new MemoryStream(imgData);
        //    //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //    //response.Content = new StreamContent(ms);
        //    //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        //    //return response;
        //}

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            HttpClient client = new HttpClient();
            var id = (new Random()).NextDouble();
            var rand = (new Random()).NextDouble();

            var settings = 
                JsonConvert.DeserializeAnonymousType(ConfigurationManager.AppSettings["ServerSettings"],
                new[]
                {
                    new
                    {
                        PortName = "",
                        Value = 0
                    }
                }).ToDictionary(k => k.PortName, v => v.Value);

            var result = await client.GetAsync($"http://localhost:{settings["CameraServerInitPort"]}/get?id={id}&r={rand}");

            return Json(new
            {
                id,
                status = result.StatusCode
            });
        }
    }
}
