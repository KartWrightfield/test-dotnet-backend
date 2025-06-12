using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Core.Interfaces.Services;

namespace UE.PostOffice.Api.Controllers
{
    [Route("api/[controller]")]
    public class DespatchDateController(IDespatchDateService despatchDateService) : Controller
    {
        [HttpGet]
        public ActionResult<DespatchDate> Get([FromQuery] List<int> productIds, DateTime orderDate)
        {
            try
            {
                var despatchDate = despatchDateService.CalculateDespatchDate(productIds, orderDate);
                
                return Ok(new DespatchDate { Date = despatchDate });
            }
            catch (Exception e)
            {
                return StatusCode(500, "Something went wrong trying to get the despatch date");
            }
        }
    }
}
