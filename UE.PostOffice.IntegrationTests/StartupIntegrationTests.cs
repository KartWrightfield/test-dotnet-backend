using System;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UE.PostOffice.Api;
using UE.PostOffice.Api.Model;
using UE.PostOffice.Api.Validators;
using UE.PostOffice.Core.Configuration;
using UE.PostOffice.Core.Interfaces.Data;
using Xunit;

namespace UE.PostOffice.IntegrationTests;

public class StartupIntegrationTests
{
    private readonly IServiceProvider _serviceProvider;

    public StartupIntegrationTests()
    {
        var webHostBuilder = new WebHostBuilder()
            .UseStartup<Startup>()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
            });
        
        var testServer = new TestServer(webHostBuilder);
        _serviceProvider = testServer.Services;
    }

    [Fact]
    public void DespatchSettings_ShouldBeConfiguredCorrectly()
    {
        //Act
        var settings = _serviceProvider.GetService<IOptions<DespatchSettings>>();
        
        //Assert
        Assert.NotNull(settings.Value);
        Assert.NotEqual(0, settings.Value.WeekendsSaturdayDelay);
        Assert.NotEqual(0, settings.Value.WeekendsSundayDelay);
    }

    [Fact]
    public void DatabaseContext_ShouldBeRegisteredAsScoped()
    {
        //Arrange
        var scope1 = _serviceProvider.CreateScope();
        var scope2 = _serviceProvider.CreateScope();
        
        //Act
        var context1 = scope1.ServiceProvider.GetService<IDbContext>();
        var context2 = scope1.ServiceProvider.GetService<IDbContext>();
        var context3 = scope2.ServiceProvider.GetService<IDbContext>();
        
        //Assert
        Assert.NotNull(context1);
        Assert.NotNull(context2);
        Assert.NotNull(context3);
        Assert.Same(context1, context2);
        Assert.NotSame(context1, context3);
    }

    [Fact]
    public void ValidatorRegistration_ShouldResolveAllValidators()
    {
        // Act
        var validator = _serviceProvider.GetRequiredService<IValidator<DespatchDateRequest>>();

        // Assert
        Assert.NotNull(validator);
        Assert.IsType<DespatchDateRequestValidator>(validator);

    }
}