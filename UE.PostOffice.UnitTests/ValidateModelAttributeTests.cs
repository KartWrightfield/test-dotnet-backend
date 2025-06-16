using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using UE.PostOffice.Api.Filters;
using UE.PostOffice.Api.Model;
using Xunit;

namespace UE.PostOffice.UnitTests;

public class ValidateModelAttributeTests
{
    private readonly ValidateModelAttribute _filterUnderTest;
    private readonly ActionExecutingContext _context;
    private readonly Mock<ActionExecutionDelegate> _next;

    private class TestModel
    {
        public string? Property { get; set; }
    }
    
    public ValidateModelAttributeTests()
    {
        _filterUnderTest = new ValidateModelAttribute();
        _next = new Mock<ActionExecutionDelegate>();

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());
        
        _context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>()!,
            new Mock<Controller>().Object);
    }

    #region ASP.NET Model Validation Tests
    
    [Fact]
    public void Given_ValidModel_When_OnActionExecutionAsync_Then_ExecutionIsNotShortCircuited()
    {
        //Arrange
        _context.ModelState.Clear();
        
        //Act
        _filterUnderTest.OnActionExecuting(_context);
        
        //Assert
        Assert.Null(_context.Result);
    }
    
    [Fact]
    public void Given_InvalidModelState_When_OnActionExecutionAsync_Then_ReturnsBadRequest()
    {
        // Arrange
        _context.ModelState.AddModelError("test", "test error");

        // Act
        _filterUnderTest.OnActionExecuting(_context);

        // Assert
        Assert.IsType<BadRequestObjectResult>(_context.Result);
        _next.Verify(n => n(), Times.Never);
    }
    
    [Fact]
    public void Given_NullModel_When_OnActionExecuting_Then_ReturnsBadRequest()
    {
        // Arrange
        _context.ActionArguments.Add("model", null);
        _context.ModelState.AddModelError("model", "Request cannot be null");

        // Act
        _filterUnderTest.OnActionExecuting(_context);

        // Assert
        Assert.IsType<BadRequestObjectResult>(_context.Result);
    }
    
    #endregion
    
    #region FluentValidation Tests
    
    [Fact]
    public void Given_ModelWithFluentValidator_When_ValidationFails_Then_ReturnsBadRequest()
    {
        // Arrange
        var testModel = new TestModel();
        var validator = new Mock<IValidator>();
        validator.Setup(v => v.Validate(It.IsAny<ValidationContext<object>>()))
            .Returns(new ValidationResult([new ValidationFailure("Property", "Error")]));
    
        var services = new Mock<IServiceProvider>();
        services.Setup(s => s.GetService(It.IsAny<Type>())).Returns(validator.Object);
        _context.HttpContext.RequestServices = services.Object;
        _context.ActionArguments.Add("model", testModel);

        // Act
        _filterUnderTest.OnActionExecuting(_context);

        // Assert
        var result = Assert.IsType<BadRequestObjectResult>(_context.Result);
        Assert.NotNull(result.Value);
    }
    
    [Fact]
    public void Given_ModelWithFluentValidator_When_ValidationPasses_Then_ContinuesExecution()
    {
        // Arrange
        var request = new DespatchDateRequest { ProductIds = [1], OrderDate = DateTime.Now };
        var validator = new Mock<IValidator>();
        validator.Setup(v => v.Validate(It.IsAny<ValidationContext<object>>()))
            .Returns(new ValidationResult()); // Empty ValidationResult means validation passed
    
        var services = new Mock<IServiceProvider>();
        services.Setup(s => s.GetService(It.IsAny<Type>())).Returns(validator.Object);
        _context.HttpContext.RequestServices = services.Object;
        _context.ActionArguments.Add("request", request);

        // Act
        _filterUnderTest.OnActionExecuting(_context);

        // Assert
        Assert.Null(_context.Result); // Null Result means the pipeline continues
        validator.Verify(v => v.Validate(It.IsAny<ValidationContext<object>>()), Times.Once);
    }
    
    #endregion
}