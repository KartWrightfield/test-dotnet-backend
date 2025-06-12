namespace UE.PostOffice.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Model;

    [Route("api/[controller]")]
    public class DespatchDateController(IDbContext dbContext) : Controller
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
            if (maxLeadTime.DayOfWeek == DayOfWeek.Saturday)
            {
                return new DespatchDate { Date = maxLeadTime.AddDays(2) };
            }
            else if (maxLeadTime.DayOfWeek == DayOfWeek.Sunday) return new DespatchDate { Date = maxLeadTime.AddDays(1) };
            else return new DespatchDate { Date = maxLeadTime };
        }
    }
}
