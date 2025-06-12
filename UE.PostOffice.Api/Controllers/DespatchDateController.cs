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
        public DespatchDate Get(List<int> productIds, DateTime orderDate)
        {
            DateTime maxLeadTime = orderDate;
            foreach (var ID in productIds)
            {
                var s = dbContext.Products.Single(x => x.ProductId == ID).SupplierId;
                var lt = dbContext.Suppliers.Single(x => x.SupplierId == s).LeadTime;
                if (orderDate.AddDays(lt) > maxLeadTime)
                    maxLeadTime = orderDate.AddDays(lt);
            }

            return new DespatchDate { Date = AdjustDateForWeekend(maxLeadTime) };
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
