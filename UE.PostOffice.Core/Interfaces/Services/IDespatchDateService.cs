namespace UE.PostOffice.Core.Interfaces.Services;

public interface IDespatchDateService
{
    Task<DateTime> CalculateDespatchDate(List<int> productIds, DateTime orderDate);
}