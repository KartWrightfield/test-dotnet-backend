using System;
using Microsoft.AspNetCore.Mvc;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Core.Interfaces.Services;

namespace UE.PostOffice.Api.Controllers
{
    [Route("api/[controller]")]
    public class DespatchDateController(IDespatchDateService despatchDateService) : Controller
    {
        [HttpGet]
        public ActionResult<DespatchDate> Get([FromQuery] DespatchDateRequest request)
        {
            try
            {
                var despatchDate = despatchDateService.CalculateDespatchDate(request.ProductIds, request.OrderDate);
                
                return Ok(new DespatchDate { Date = despatchDate });
            }
            catch (Exception e)
            {
                return StatusCode(500, "Something went wrong trying to get the despatch date");
            }
        }
    }
}
