namespace UE.PostOffice.Core.Interfaces.Services;

public interface IDespatchDateService
{
    DateTime CalculateDespatchDate(List<int> productIds, DateTime orderDate);
}