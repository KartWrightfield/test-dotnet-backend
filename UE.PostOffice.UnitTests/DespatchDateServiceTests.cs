using Microsoft.Extensions.Options;
using Moq;
using UE.PostOffice.Core.Configuration;
using UE.PostOffice.Core.Interfaces.Repositories;
using UE.PostOffice.Core.Services;
using Xunit;

namespace UE.PostOffice.UnitTests;

public class DespatchDateServiceTests
{
    private readonly DespatchDateService _serviceUnderTest;

    private readonly Mock<ISupplierRepository> _mockSupplierRepository;

    private static readonly DateTime Monday = new(2025, 6, 9);
    private static readonly DateTime Tuesday = new(2025, 6, 10);
    private static readonly DateTime Wednesday = new(2025, 6, 11);
    private static readonly DateTime Thursday = new(2025, 6, 12);
    private static readonly DateTime Friday = new(2025, 6, 13);
    private static readonly DateTime Saturday = new(2025, 6, 14);
    private static readonly DateTime Sunday = new(2025, 6, 15);
    
    private static readonly DateTime WeekMonday = new(2025, 6, 16);
    private static readonly DateTime WeekTuesday = new(2025, 6, 17);
    private static readonly DateTime WeekWednesday = new(2025, 6, 18);
    private static readonly DateTime WeekThursday = new(2025, 6, 19);
    

    public DespatchDateServiceTests()
    {
        _mockSupplierRepository = new Mock<ISupplierRepository>();
        
        var despatchSettings = new DespatchSettings
        {
            WeekendsSaturdayDelay = 2,
            WeekendsSundayDelay = 1
        };
        var despatchSettings1 = Options.Create(despatchSettings);
        
        _serviceUnderTest = new DespatchDateService(_mockSupplierRepository.Object, despatchSettings1);
    }
    
    #region Single-Day Lead Time Tests
    
    /// <summary>
    /// How to read this test data:
    /// { Monday, Tuesday }: For a one-day lead-time supplier, a Monday order should result in a Tuesday dispatch
    /// </summary>
    public static TheoryData<DateTime, DateTime> OneDayLeadTimeTestData =>
        new()
        {
            { Monday, Tuesday }, 
            { Tuesday,  Wednesday },
            { Wednesday, Thursday },
            { Thursday, Friday },
            { Friday, WeekMonday },
            { Saturday, WeekTuesday },
            { Sunday, WeekTuesday }
        };


    [Theory]
    [MemberData(nameof(OneDayLeadTimeTestData))]
    public async Task Given_MaxLeadTimeOf1_Then_CorrectDespatchDateIsReturned(DateTime orderDate,
        DateTime expectedDespatchDate)
    {
        //Arrange
        const int maxLeadTime = 1;
        List<int> productIds = [1, 2, 3];
        _mockSupplierRepository
            .Setup(x => x.GetMaxLeadTimeForProducts(productIds))
            .ReturnsAsync(maxLeadTime);
        
        //Act
        var result = await _serviceUnderTest.CalculateDespatchDate(productIds, orderDate);
        
        //Assert
        Assert.Equal(expectedDespatchDate.Date, result.Date);
        _mockSupplierRepository.Verify(x =>  x.GetMaxLeadTimeForProducts(productIds), Times.Once);
    }
    
    #endregion
    
    #region Multi-Day Lead-Time Tests

    /// <summary>
    /// How to read this test data:
    /// { Monday, Thursday }: For a three-day lead-time supplier, a Monday order should result in a Thursday dispatch
    /// </summary>
    public static TheoryData<DateTime, DateTime> ThreeDayLeadTimeTestData =>
        new()
        {
            { Monday, Thursday },
            { Tuesday, Friday },
            { Wednesday, WeekMonday },
            { Thursday, WeekTuesday },
            { Friday, WeekWednesday },
            { Saturday, WeekThursday },
            { Sunday, WeekThursday },
        };
    
    [Theory]
    [MemberData(nameof(ThreeDayLeadTimeTestData))]
    public async Task Given_MaxLeadTimeOf3_Then_CorrectDespatchDateIsReturned(DateTime orderDate,
        DateTime expectedDespatchDate)
    {
        //Arrange
        const int maxLeadTime = 3;
        List<int> productIds = [4, 5, 6];
        _mockSupplierRepository
            .Setup(x => x.GetMaxLeadTimeForProducts(productIds))
            .ReturnsAsync(maxLeadTime);
        
        //Act
        var result = await _serviceUnderTest.CalculateDespatchDate(productIds, orderDate);
        
        //Assert
        Assert.Equal(expectedDespatchDate.Date, result.Date);
        _mockSupplierRepository.Verify(x =>  x.GetMaxLeadTimeForProducts(productIds), Times.Once);
    }

    #endregion

    #region Multi-Week Lead-Time Tests

    private static readonly DateTime TwoWeeksTuesday = new(2025, 6, 24);
    private static readonly DateTime TwoWeeksWednesday = new(2025, 6, 25);
    private static readonly DateTime TwoWeeksThursday = new(2025, 6, 26);
    private static readonly DateTime TwoWeeksFriday = new(2025, 6, 27);
    
    private static readonly DateTime ThreeWeeksMonday = new(2025, 6, 30);
    private static readonly DateTime ThreeWeeksTuesday = new(2025, 7, 1);
    
    /// <summary>
    /// How to read this test data:
    /// { Monday, TwoWeeksTuesday }: For an eleven-day lead-time supplier, a Monday order should result in a despatch two weeks Tuesday
    /// </summary>
    public static TheoryData<DateTime, DateTime> ElevenDayLeadTimeTestData =>
        new()
        {
            { Monday, TwoWeeksTuesday },
            { Tuesday, TwoWeeksWednesday },
            { Wednesday, TwoWeeksThursday },
            { Thursday, TwoWeeksFriday },
            { Friday, ThreeWeeksMonday },
            { Saturday, ThreeWeeksTuesday },
            { Sunday, ThreeWeeksTuesday },
        };
    
    [Theory]
    [MemberData(nameof(ElevenDayLeadTimeTestData))]
    public async Task Given_MaxLeadTimeOf11_Then_CorrectDespatchDateIsReturned(DateTime orderDate,
        DateTime expectedDespatchDate)
    {
        //Arrange
        const int maxLeadTime = 11;
        List<int> productIds = [7, 8, 9];
        _mockSupplierRepository
            .Setup(x => x.GetMaxLeadTimeForProducts(productIds))
            .ReturnsAsync(maxLeadTime);
        
        //Act
        var result = await _serviceUnderTest.CalculateDespatchDate(productIds, orderDate);
        
        //Assert
        Assert.Equal(expectedDespatchDate.Date, result.Date);
        _mockSupplierRepository.Verify(x =>  x.GetMaxLeadTimeForProducts(productIds), Times.Once);
    }
    
    #endregion

    #region Other Tests
    
    [Fact]
    public async Task
        Given_MaxLeadTimeOf11_When_OrderPlacedInLateDecember_Then_CorrectDespatchDateInNextYearIsReturned()
    {
        //Arrange
        DateTime orderDate = new(2024, 12, 23); //Monday
        DateTime expectedDespatchDate = new(2025, 1, 7); //Two weeks Tuesday
        
        const int maxLeadTime = 11;
        List<int> productIds = [4, 5, 6];
        _mockSupplierRepository
            .Setup(x => x.GetMaxLeadTimeForProducts(productIds))
            .ReturnsAsync(maxLeadTime);
        
        //Act
        var result = await _serviceUnderTest.CalculateDespatchDate(productIds, orderDate);
        
        //Assert
        Assert.Equal(expectedDespatchDate.Date, result.Date);
        _mockSupplierRepository.Verify(x =>  x.GetMaxLeadTimeForProducts(productIds), Times.Once);
    }

    #endregion
}