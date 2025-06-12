using UE.PostOffice.Data;

namespace UE.PostOffice.Tests
{
    using System;
    using System.Collections.Generic;
    using Api.Controllers;
    using Shouldly;
    using Xunit;

    public class PostOfficeTests
    {
        private readonly DespatchDateController _controllerUnderTest;

        public PostOfficeTests()
        {
            IDbContext dbContext = new DbContext();
            _controllerUnderTest = new DespatchDateController(dbContext);
        }
        
        [Fact]
        public void OneProductWithLeadTimeOfOneDay()
        {
            var date = _controllerUnderTest.Get(new List<int>() {1}, DateTime.Now);
            date.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(1));
        }

        [Fact]
        public void OneProductWithLeadTimeOfTwoDay()
        {
            var date = _controllerUnderTest.Get(new List<int>() { 2 }, DateTime.Now);
            date.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(2));
        }

        [Fact]
        public void OneProductWithLeadTimeOfThreeDay()
        {
            var date = _controllerUnderTest.Get(new List<int>() { 3 }, DateTime.Now);
            date.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(3));
        }

        [Fact]
        public void SaturdayHasExtraTwoDays() {

            var date = _controllerUnderTest.Get(new List<int>() { 1 }, new DateTime(2018,1,26));
            date.Date.ShouldBe(new DateTime(2018, 1, 26).Date.AddDays(3));
        }

        [Fact]
        public void SundayHasExtraDay()
        {
            var date = _controllerUnderTest.Get(new List<int>() { 3 }, new DateTime(2018, 1, 25));
            date.Date.ShouldBe(new DateTime(2018, 1, 25).Date.AddDays(4));
        }
    }
}
