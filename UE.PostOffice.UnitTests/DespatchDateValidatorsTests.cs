using FluentValidation.TestHelper;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Api.Validators;
using Xunit;

namespace UE.PostOffice.UnitTests;

public class DespatchDateValidatorsTests
{
    private readonly DespatchDateRequestValidator _validatorUnderTest = new();

    [Fact]
    public void ShouldFailWhenProductIdsEmpty()
    {
        var request = new DespatchDateRequest()
        {
            ProductIds = [],
            OrderDate = DateTime.Now
        };

        var result = _validatorUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductIds);
    }
}