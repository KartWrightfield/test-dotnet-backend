using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shouldly;
using UE.PostOffice.Api.Controllers;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Core.Configuration;
using UE.PostOffice.Core.Interfaces.Data;
using UE.PostOffice.Core.Interfaces.Repositories;
using UE.PostOffice.Core.Interfaces.Services;
using UE.PostOffice.Core.Services;
using UE.PostOffice.Data.Context;
using UE.PostOffice.Data.Repositories;
using Xunit;

namespace UE.PostOffice.IntegrationTests
{
    public class PostOfficeTests
    {
        private readonly DespatchDateController _controllerUnderTest;

        private static class TestDates
        {
            public static readonly DateTime Monday = new DateTime(2025, 6, 9);
            public static readonly DateTime Tuesday = new DateTime(2025, 6, 10);
            public static readonly DateTime Wednesday = new DateTime(2025, 6, 11);
            public static readonly DateTime Thursday = new DateTime(2025, 6, 12);
            public static readonly DateTime Friday = new DateTime(2025, 6, 13);
            public static readonly DateTime Saturday = new DateTime(2025, 6, 14);
            public static readonly DateTime Sunday = new DateTime(2025, 6, 15);
        }

        public PostOfficeTests()
        {
            IDbContext dbContext = new DbContext();
            var despatchSettings = new DespatchSettings
            {
                WeekendsSaturdayDelay = 2,
                WeekendsSundayDelay = 1
            };
            var options = Options.Create(despatchSettings);
            
            ISupplierRepository supplierRepository = new SupplierRepository(dbContext);
            IDespatchDateService despatchDateService = new DespatchDateService(supplierRepository, options);
            
            _controllerUnderTest = new DespatchDateController(despatchDateService);
        }
        
        [Fact]
        public async Task 
            Given_OneProductWithLeadTimeOfOneDay_When_OrderDateIsMonday_Then_ResultShouldBeTheDateOfTheFollowingTuesday()
        {
            var testDate = TestDates.Monday;
            var request = new DespatchDateRequest { ProductIds = [1], OrderDate = testDate };
            
            var response = await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(1));
        }

        [Fact]
        public async Task 
            Given_OneProductWithLeadTimeOfOneDay_When_OrderDateIsFriday_Then_ResultShouldBeTheDateOfTheFollowingMonday()
        {
            var testDate = TestDates.Friday;
            var request = new DespatchDateRequest { ProductIds = [1],  OrderDate = testDate };
            
            var response = await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(3));
        }

        [Fact]
        public async Task
            Given_OneProductWithLeadTimeOfOneDay_When_OrderDateIsSaturday_ThenResultShouldBeTheDateOfTheFollowingTuesday()
        {
            var testDate = TestDates.Saturday;
            var request = new DespatchDateRequest { ProductIds = [1], OrderDate = testDate };
            
            var response = await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(3));
        }

        [Fact]
        public async Task 
            Given_OneProductWithLeadTimeOfTwoDays_When_OrderDateIsTuesday_Then_ResultShouldBeTheDateOfTheFollowingThursday()
        {
            var testDate = TestDates.Tuesday;
            var request = new  DespatchDateRequest { ProductIds = [2], OrderDate = testDate };
            
            var response = await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(2));
        }

        [Fact]
        public async Task
            Given_OneProductWithLeadTimeOfTwoDays_When_OrderDateIsFriday_ThenResultShouldBeTheDateOfTheFollowingTuesday()
        {
            var testDate = TestDates.Friday;
            var request = new DespatchDateRequest { ProductIds = [2], OrderDate = testDate };
            
            var response =  await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(4));
        }

        [Fact]
        public async Task
            Given_OneProductWithLeadTimeOfTwoDays_When_OrderDateIsSunday_Then_ResultShouldBeTheDateOfTheFollowingWednesday()
        {
            var testDate = TestDates.Sunday;
            var request = new DespatchDateRequest { ProductIds = [2], OrderDate = testDate };
            
            var response = await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(3));
        }

        [Fact]
        public async Task
            Given_ProductsWithMaxLeadTimeOfOneDay_When_OrderDateIsWednesday_Then_ResultShouldBeTheDateOfTheFollowingThursday()
        {
            var testDate = TestDates.Wednesday;
            var request = new DespatchDateRequest { ProductIds = [1, 4], OrderDate = testDate };
            
            var response = await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(1));
        }

        [Fact]
        public async Task
            Given_ProductsWithMixedLeadTimesButAMaxOfThreeDays_When_OrderDateIsMonday_Then_ResultShouldBeTheDateOfTheFollowingThursday()
        {
            var testDate = TestDates.Monday;
            var request = new DespatchDateRequest { ProductIds = [3, 4], OrderDate = testDate };
            
            var response =  await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(3));
        }
        
        [Fact]
        public async Task
            Given_ProductsWithMixedLeadTimesButAMaxOfThreeDays_When_OrderDateIsThursday_Then_ResultShouldBeTheDateOfTheFollowingTuesday()
        {
            var testDate = TestDates.Thursday;
            var request = new DespatchDateRequest { ProductIds = [3, 4], OrderDate = testDate };
            
            var response =  await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(5));
        }

        [Fact]
        public async Task
            Given_ProductsWithMaxLeadTimeOfSixDays_When_OrderDateIsWednesday_Then_ResultShouldBeTheDateOfTheFollowingThursday()
        {
            var testDate = TestDates.Wednesday;
            var request = new DespatchDateRequest { ProductIds = [9, 1, 4], OrderDate = testDate };
            
            var response =  await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(8));
        }
        
        [Fact]
        public async Task
            Given_ProductsWithMaxLeadTimeOfSixDays_When_OrderDateIsFriday_Then_ResultShouldBeTheDateOfTheSecondFollowingMonday()
        {
            var testDate = TestDates.Friday;
            var request = new DespatchDateRequest { ProductIds = [9, 1, 4], OrderDate = testDate };
            
            var response =  await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(10));
        }

        [Fact]
        public async Task
            Given_ProductsWithMaxLeadTimeOfThirteenDays_When_OrderDateIsFriday_Then_ResultShouldBeTheDateOfTheThirdFollowingWednesday()
        {
            var testDate = TestDates.Friday;
            var request = new  DespatchDateRequest { ProductIds = [9, 10, 4], OrderDate = testDate };
            
            var response =  await _controllerUnderTest.Get(request);
            
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(testDate.AddDays(19));
        }
    }
}
