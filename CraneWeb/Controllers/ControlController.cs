﻿using CraneWeb.Data;
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
        [HttpPost]
        public IHttpActionResult Operate(List<ControlboardOperation> ops)
        {
            Driver.OperateCrane(ops);
            return Ok();
        }

        [HttpPut, Route("api/control/off")]
        public IHttpActionResult Off()
        {
            Driver.Off();
            return Ok();
        }

        [HttpGet, Route("api/control/ports")]
        public IHttpActionResult GetAvailablePorts()
        {
            return Json(SerialLib.Driver.EnumeratePorts());
        }

        [HttpGet, Route("api/actions/south")]
        public IHttpActionResult GetSouthchipActions()
        {
            using (CraneDbContext context = new CraneDbContext())
            {
                return Json(context.CraneOperations.Where(s => s.ActionSource == ActionSource.SouthChip).ToList());
            }
        }

        [HttpGet, Route("api/actions/north")]
        public IHttpActionResult GetNorthChipActions()
        {
            using (CraneDbContext context = new CraneDbContext())
            {
                return Json(context.CraneOperations.Where(s => s.ActionSource == ActionSource.NorthChip).ToList());
            }
        }

        [HttpGet, Route("api/actions/all")]
        public IHttpActionResult GetAllActions()
        {
            using (CraneDbContext context = new CraneDbContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                var operations = context.CraneOperations
                    .ToDictionary(k => k.OpCode.ToString(), v => v);

                return Json(operations);
            }
        }
    }
}
