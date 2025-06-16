using Microsoft.Extensions.Options;
using UE.PostOffice.Core.Configuration;
using UE.PostOffice.Core.Interfaces.Repositories;
using UE.PostOffice.Core.Interfaces.Services;

namespace UE.PostOffice.Core.Services;

/// <summary>
/// Service that provides functionality for determining despatch dates for orders.
/// </summary>
public class DespatchDateService(ISupplierRepository supplierRepository, IOptions<DespatchSettings> settings)
    : IDespatchDateService
{
    /// <summary>
    /// Calculates the despatch date for an order based on the provided product IDs and order date.
    /// Takes into consideration weekends, business days, and the lead time required by the suppliers.
    /// </summary>
    /// <param name="productIds">A list of product IDs for the order.</param>
    /// <param name="orderDate">The date the order was placed.</param>
    /// <returns>The calculated despatch date.</returns>
    public async Task<DateTime> CalculateDespatchDate(List<int> productIds, DateTime orderDate)
    {
        var adjustedOrderDate = AdjustOrderDateForWeekend(orderDate);
        var maxLeadTime = await supplierRepository.GetMaxLeadTimeForProducts(productIds);
        
        return AddBusinessDays(adjustedOrderDate, maxLeadTime);
    }

    /// <summary>
    /// Adds the specified number of business days to the given date.
    /// Excludes weekends (Saturday and Sunday) from the calculation.
    /// </summary>
    /// <param name="orderDate">The starting date from which the business days will be added.</param>
    /// <param name="daysRequiredToFulfil">The number of business days to add to the starting date.</param>
    /// <returns>The resulting date after adding the specified number of business days.</returns>
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

    /// <summary>
    /// Adjusts the order date if it falls on a weekend by adding a delay, as specified in the settings.
    /// Saturdays and Sundays are adjusted with their respective configured delays to align with business days.
    /// </summary>
    /// <param name="orderDate">The original order date to be adjusted if it falls on a weekend.</param>
    /// <returns>The adjusted order date, accounting for weekend-specific delays.</returns>
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