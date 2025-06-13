using FluentValidation.TestHelper;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Api.Validators;
using Xunit;

namespace UE.PostOffice.UnitTests;

public class DespatchDateValidatorsTests
{
    private readonly DespatchDateRequestValidator _validatorUnderTest = new();

    [Fact]
    public void Given_EmptyProductIds_When_ValidatingRequest_Then_ValidationFails()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [],
            OrderDate = DateTime.UtcNow
        };

        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductIds);
    }

    [Fact]
    public void Given_DuplicateProductIds_When_ValidatingRequest_Then_ValidationFails()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [95, 1, 1, 91],
            OrderDate = DateTime.UtcNow
        };
        
        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductIds);
    }

    [Fact]
    public void Given_NegativeProductIds_When_ValidatingRequest_Then_ValidationFails()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [95, -4, 43],
            OrderDate = DateTime.UtcNow
        };

        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductIds);
    }
    
    [Fact]
    public void Given_ZeroValueProductIds_WhenValidatingRequest_Then_ValidationFails()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [3, 12, 0],
            OrderDate = DateTime.UtcNow
        };

        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductIds);
    }

    [Fact]
    public void Given_DefaultValueOrderDate_When_ValidatingRequest_Then_ValidationFails()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [3, 12, 64],
            OrderDate = new DateTime()
        };

        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.OrderDate);
    }

    [Fact]
    public void Given_OrderDateInFuture_When_ValidatingRequest_Then_ValidationFails()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [3, 12, 14],
            OrderDate = DateTime.UtcNow.AddDays(3)
        };

        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.OrderDate);
    }

    [Fact]
    public void Given_ValidParameters_When_ValidatingRequest_Then_ValidationPasses()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [541, 24, 5],
            OrderDate = DateTime.UtcNow.AddDays(-1)
        };
        
        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void Given_ValidBoundaryValueProductIds_When_ValidatingRequest_Then_ValidationPasses(int productId)
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [productId],
            OrderDate = DateTime.UtcNow
        };
        
        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}