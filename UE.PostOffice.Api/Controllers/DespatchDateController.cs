using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Core.Interfaces.Services;

namespace UE.PostOffice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DespatchDateController(IDespatchDateService despatchDateService) : Controller
    {
        /// <summary>
        /// Calculates the estimated despatch date of an order 
        /// </summary>
        /// <param name="request">Request containing product IDs and the order date</param>
        /// <returns>The calculated despatch date for the order</returns>
        /// <response code="200">Returns the calculated despatch date</response>
        /// <response code="500">If there was an unexpected error during date calculation</response>
        [HttpGet]
        [ProducesResponseType(typeof(DespatchDate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
