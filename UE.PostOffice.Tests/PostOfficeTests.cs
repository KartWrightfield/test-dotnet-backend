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

namespace UE.PostOffice.Tests
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
            var response = _controllerUnderTest.Get([1], DateTime.Now);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(1));
        }

        [Fact]
        public void OneProductWithLeadTimeOfTwoDay()
        {
            var response = _controllerUnderTest.Get([2], DateTime.Now);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(2));
        }

        [Fact]
        public void OneProductWithLeadTimeOfThreeDay()
        {
            var response = _controllerUnderTest.Get([3], DateTime.Now);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(3));
        }

        [Fact]
        public void SaturdayHasExtraTwoDays() {

            var response = _controllerUnderTest.Get([1], new DateTime(2018,1,26));
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.ShouldBe(new DateTime(2018, 1, 26).Date.AddDays(3));
        }

        [Fact]
        public void SundayHasExtraDay()
        {
            var response = _controllerUnderTest.Get([3], new DateTime(2018, 1, 25));
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var despatchDate = Assert.IsType<DespatchDate>(okResult.Value);
            despatchDate.Date.ShouldBe(new DateTime(2018, 1, 25).Date.AddDays(4));
        }
    }
}
