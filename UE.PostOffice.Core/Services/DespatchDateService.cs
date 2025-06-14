using Microsoft.Extensions.Options;
using UE.PostOffice.Core.Configuration;
using UE.PostOffice.Core.Interfaces.Repositories;
using UE.PostOffice.Core.Interfaces.Services;

namespace UE.PostOffice.Core.Services;

public class DespatchDateService(ISupplierRepository supplierRepository, IOptions<DespatchSettings> settings) : IDespatchDateService
{
    public async Task<DateTime> CalculateDespatchDate(List<int> productIds, DateTime orderDate)
    {
        var maxLeadTime = await supplierRepository.GetMaxLeadTimeForProducts(productIds);
        var rawDespatchDate = orderDate.AddDays(maxLeadTime);
        
        return AdjustDateForWeekend(rawDespatchDate);
    }
    
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