
using Serilog.Core;
using Serilog.Events;

namespace FoodSafetyTracker.Web.Logging;

/// <summary>
/// Enricher do Serilog responsável por adicionar automaticamente o nome
/// do usuário logado a todos os logs da aplicação.
/// Usa IServiceProvider para resolver o IHttpContextAccessor a cada log,
/// evitando o problema de serviço scoped dentro de contexto singleton.
/// Se não houver usuário logado registra "Anonymous".
/// Utilizado por: Program.cs na configuração do Serilog.
/// </summary>
public class UserNameEnricher : ILogEventEnricher
{
    private readonly IServiceProvider _serviceProvider;
    
    public UserNameEnricher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Adiciona a propriedade UserName ao evento de log.
    /// Cria um escopo temporário para resolver o IHttpContextAccessor
    /// de forma segura dentro do contexto singleton do Serilog.
    /// </summary>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var httpContextAccessor = serviceScope.ServiceProvider
            .GetRequiredService<IHttpContextAccessor>();

        var userName = httpContextAccessor.HttpContext?.User?.Identity?.Name
                       ?? "Anonymous";

        var userNameProperty = propertyFactory.CreateProperty("UserName", userName);

        logEvent.AddPropertyIfAbsent(userNameProperty);
    }
}