using FoodSafetyTracker.Web.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace FoodSafetyTracker.Tests.Authorization;

public class UserNameEnricherTests
{
    /// <summary>
    /// Cria um IServiceProvider configurado com IHttpContextAccessor
    /// apontando para um HttpContext sem usuário logado.
    /// </summary>
    private IServiceProvider CreateServiceProviderWithoutUser()
    {
        var httpContext = new DefaultHttpContext();

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IHttpContextAccessor>(httpContextAccessor);

        return serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Cria um LogEvent vazio para usar nos testes do enricher.
    /// </summary>
    private static LogEvent CreateEmptyLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Information,
            exception: null,
            new MessageTemplate("Test", Enumerable.Empty<MessageTemplateToken>()),
            Enumerable.Empty<LogEventProperty>());
    }

    /// <summary>
    /// Implementação simples de ILogEventPropertyFactory para uso nos testes.
    /// Cria propriedades de log com valores escalares simples.
    /// </summary>
    private class LogEventPropertyFactory : ILogEventPropertyFactory
    {
        public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false)
        {
            return new LogEventProperty(name, new ScalarValue(value));
        }
    }

    [Fact]
    public void UserNameEnricher_WhenNoUserIsLoggedIn_ShouldEnrichWithAnonymous()
    {
        // Arrange — cria o enricher com um HttpContext sem usuário logado
        var serviceProvider = CreateServiceProviderWithoutUser();
        var enricher = new UserNameEnricher(serviceProvider);
        var logEvent = CreateEmptyLogEvent();
        var propertyFactory = new LogEventPropertyFactory();

        // Act — aplica o enricher ao evento de log
        enricher.Enrich(logEvent, propertyFactory);

        // Assert — verifica que a propriedade UserName existe e é Anonymous
        Assert.True(logEvent.Properties.ContainsKey("UserName"));
        Assert.Equal("\"Anonymous\"", logEvent.Properties["UserName"].ToString());
    }

    [Fact]
    public void UserNameEnricher_WhenUserIsLoggedIn_ShouldEnrichWithUserName()
    {
        // Arrange — cria um HttpContext com um usuário autenticado
        var httpContext = new DefaultHttpContext();
        httpContext.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                new[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Name,
                        "inspector@foodsafety.ie")
                },
                authenticationType: "TestAuthentication"
            )
        );

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IHttpContextAccessor>(httpContextAccessor);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var enricher = new UserNameEnricher(serviceProvider);
        var logEvent = CreateEmptyLogEvent();
        var propertyFactory = new LogEventPropertyFactory();

        // Act — aplica o enricher ao evento de log
        enricher.Enrich(logEvent, propertyFactory);

        // Assert — verifica que o UserName é o do usuário logado
        Assert.True(logEvent.Properties.ContainsKey("UserName"));
        Assert.Equal("\"inspector@foodsafety.ie\"", logEvent.Properties["UserName"].ToString());
    }
    
}