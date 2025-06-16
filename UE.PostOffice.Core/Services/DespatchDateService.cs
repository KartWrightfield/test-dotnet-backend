using Microsoft.Extensions.Options;
using UE.PostOffice.Core.Configuration;
using UE.PostOffice.Core.Interfaces.Repositories;
using UE.PostOffice.Core.Interfaces.Services;

namespace UE.PostOffice.Core.Services;

public class DespatchDateService(ISupplierRepository supplierRepository, IOptions<DespatchSettings> settings) : IDespatchDateService
{
    public async Task<DateTime> CalculateDespatchDate(List<int> productIds, DateTime orderDate)
    {
        var adjustedOrderDate = AdjustOrderDateForWeekend(orderDate);
        var maxLeadTime = await supplierRepository.GetMaxLeadTimeForProducts(productIds);
        
        return AddBusinessDays(adjustedOrderDate, maxLeadTime);
    }

    private DateTime AddBusinessDays(DateTime orderDate, int daysRequiredToFulfil)
    {
        var despatchDate = orderDate;
        var remainingFulfilmentDaysRequired = daysRequiredToFulfil;

        while (remainingFulfilmentDaysRequired > 0)
        {
            despatchDate = despatchDate.AddDays(1);

            if (despatchDate.DayOfWeek != DayOfWeek.Saturday &&
                despatchDate.DayOfWeek != DayOfWeek.Sunday)
            {
                remainingFulfilmentDaysRequired--;
            }
        }

        return despatchDate;
    }
    
    private DateTime AdjustOrderDateForWeekend(DateTime orderDate)
    {
        return orderDate.DayOfWeek switch
        {
            DayOfWeek.Saturday => orderDate.AddDays(settings.Value.WeekendsSaturdayDelay),
            DayOfWeek.Sunday => orderDate.AddDays(settings.Value.WeekendsSundayDelay),
            _ => orderDate
        };
    }
}