
using FoodSafetyTracker.Application.Interfaces;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafetyTracker.Web.Controllers;

/// <summary>
/// Controller responsável pela exibição do dashboard principal.
/// Exibe contagens agregadas de inspeções e acompanhamentos com filtros.
/// Acessível por todos os usuários autenticados.
/// Utiliza: IDashboardService para todas as queries de agregação.
/// </summary>
[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    /// <summary>
    /// Recebe o IDashboardService e o ILogger via Dependency Injection.
    /// </summary>
    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    /// <summary>
    /// Exibe o dashboard principal com contagens agregadas.
    /// Recebe filtros opcionais de cidade e nível de risco via query string.
    /// Acessível por todos os usuários autenticados incluindo Viewer.
    /// </summary>
    public async Task<IActionResult> Index(string? cityFilter, RiskRating? riskRatingFilter)
    {
        var dashboardData = await _dashboardService.GetDashboardDataAsync(
            cityFilter,
            riskRatingFilter);

        return View(dashboardData);
    }
}