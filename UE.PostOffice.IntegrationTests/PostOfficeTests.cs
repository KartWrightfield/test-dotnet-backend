using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shouldly;
using UE.PostOffice.Api.Controllers;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Core.Configuration;
using UE.PostOffice.Core.Interfaces.Repositories;
using UE.PostOffice.Core.Interfaces.Services;
using UE.PostOffice.Core.Services;
using UE.PostOffice.Data;
using UE.PostOffice.Data.Repositories;
using Xunit;

namespace UE.PostOffice.IntegrationTests
{
    public class PostOfficeTests
    {
        private readonly DespatchDateController _controllerUnderTest;

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
        public void OneProductWithLeadTimeOfOneDay()
        {
            var request = new DespatchDateRequest { ProductIds = [1], OrderDate = DateTime.Now };
            
            var response = _controllerUnderTest.Get(request);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(1));
        }

        [Fact]
        public void OneProductWithLeadTimeOfTwoDay()
        {
            var request = new DespatchDateRequest { ProductIds = [2], OrderDate = DateTime.Now };
            
            var response = _controllerUnderTest.Get(request);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(2));
        }

        [Fact]
        public void OneProductWithLeadTimeOfThreeDay()
        {
            var request = new DespatchDateRequest { ProductIds = [3], OrderDate = DateTime.Now };
            
            var response = _controllerUnderTest.Get(request);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(3));
        }

        [Fact]
        public void SaturdayHasExtraTwoDays() 
        {
            var request = new DespatchDateRequest { ProductIds = [1], OrderDate = new DateTime(2018,1,26) };

            var response = _controllerUnderTest.Get(request);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.ShouldBe(new DateTime(2018, 1, 26).Date.AddDays(3));
        }

        [Fact]
        public void SundayHasExtraDay()
        {
            var request = new DespatchDateRequest { ProductIds = [3], OrderDate = new DateTime(2018, 1, 25) };
            
            var response = _controllerUnderTest.Get(request);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.ShouldBe(new DateTime(2018, 1, 25).Date.AddDays(4));
        }
    }
}
