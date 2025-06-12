using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UE.PostOffice.Api.Configuration;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Data;

namespace UE.PostOffice.Api.Controllers
{
    [Route("api/[controller]")]
    public class DespatchDateController(IDbContext dbContext, IOptions<DespatchSettings> settings) : Controller
    {
        [HttpGet]
        public ActionResult<DespatchDate> Get(List<int> productIds, DateTime orderDate)
        {
            try
            {
                DateTime maxLeadTime = orderDate;
                foreach (var productId in productIds)
                {
                    var productSupplierId = dbContext.Products.Single(x => x.ProductId == productId).SupplierId;
                    var supplierLeadTime = dbContext.Suppliers.Single(x => x.SupplierId == productSupplierId).LeadTime;
                    if (orderDate.AddDays(supplierLeadTime) > maxLeadTime)
                        maxLeadTime = orderDate.AddDays(supplierLeadTime);
                }

                return Ok(new DespatchDate { Date = AdjustDateForWeekend(maxLeadTime) });
            }
            catch (Exception e)
            {
                return StatusCode(500, "Something went wrong trying to get the despatch date");
            }
        }
        
        //To be relocated later
        private DateTime AdjustDateForWeekend(DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Saturday => date.AddDays(settings.Value.WeekendsSaturdayDelay),
                DayOfWeek.Sunday => date.AddDays(settings.Value.WeekendsSundayDelay),
                _ => date
            };
        } 
    }
}
