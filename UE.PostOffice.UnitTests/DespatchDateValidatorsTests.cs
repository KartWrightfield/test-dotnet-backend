using FluentValidation.TestHelper;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Api.Validators;
using Xunit;

namespace UE.PostOffice.UnitTests;

public class DespatchDateValidatorsTests
{
    private readonly DespatchDateRequestValidator _validatorUnderTest = new();

    /// <summary>
    /// Product IDs must be provided to determine supplier lead times.
    /// An empty list would make it impossible to calculate the despatch date.
    /// </summary>
    [Fact]
    [Trait("Category", "ProductIdsValidation")]
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

    /// <summary>
    /// Duplicate product IDs are not allowed as they would:
    /// 1. Potential to harm the performance of database queries.
    /// 2. Indicate a potential client-side error.
    /// </summary>
    [Fact]
    [Trait("Category", "ProductIdsValidation")]
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

    /// <summary>
    /// Negative product IDs are not allowed as there should be no products in the system with IDs less than 1
    /// </summary>
    [Fact]
    [Trait("Category", "ProductIdsValidation")]
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
    
    /// <summary>
    /// Zero value product IDs are not allowed as there should be no products in the system with IDs less than 1
    /// </summary>
    [Fact]
    [Trait("Category", "ProductIdsValidation")]
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

    /// <summary>
    /// Default/MinValue datetime not allowed as it would indicate either a clint-side error or an issue with instantiation of the incoming parameter 
    /// </summary>
    [Fact]
    [Trait("Category", "OrderDateValidation")]
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

    /// <summary>
    /// Order dates in the future are not allowed because:
    /// 1. It's impossible in our shared agreement of how time works
    /// 2. It would suggest a client-side error
    /// 3. Or, an inconsistency with how client-side and server side agree to use datetime (i.e. timezone specific or UTC)
    /// </summary>
    [Fact]
    [Trait("Category", "OrderDateValidation")]
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

    /// <summary>
    /// Order dates in the future are not allowed because:
    /// 1. It's impossible in our shared agreement of how time works
    /// 2. It would suggest a client-side error
    /// 3. Or, an inconsistency with how client-side and server side agree to use datetime (i.e. timezone specific or UTC)
    /// </summary>
    [Fact]
    [Trait("Category", "OrderDateValidation")]
    public void Given_OrderDateInvalidBoundary_When_ValidatingRequest_Then_ValidationFails()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [3, 12, 14],
            OrderDate = DateTime.UtcNow.AddSeconds(1)
        };

        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.OrderDate);
    }

    [Fact]
    public void Given_OrderDateValidBoundary_When_ValidatingRequest_Then_ValidationPasses()
    {
        var request = new DespatchDateRequest
        {
            ProductIds = [3, 12, 14],
            OrderDate = DateTime.UtcNow.AddSeconds(-1)
        };

        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
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
    [Trait("Type", "BoundaryTest")]
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