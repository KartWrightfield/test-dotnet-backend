using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Core.Interfaces.Services;

namespace UE.PostOffice.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling operations related to despatch dates.
    /// </summary>
    /// <remarks>
    /// Requires authorisation for accessing its endpoints.
    /// </remarks>
    [Authorize]
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
        /// <response code="400">If the request fails validation</response>
        /// <response code="401">If the request isn't bearing a valid auth token</response>
        /// <response code="500">If there was an unexpected error during date calculation</response>
        [HttpGet]
        [ProducesResponseType(typeof(DespatchDate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DespatchDate>> Get([FromQuery][Bind(Prefix = "")] DespatchDateRequest request)
        {
            try
            {
                var despatchDate = await despatchDateService.CalculateDespatchDate(request.ProductIds, request.OrderDate);
                
                return Ok(new DespatchDate { Date = despatchDate });
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong trying to get the despatch date");
            }
        }
    }
}
