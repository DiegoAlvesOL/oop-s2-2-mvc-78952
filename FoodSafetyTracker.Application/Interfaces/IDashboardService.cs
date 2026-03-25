using FoodSafetyTracker.Application.ViewModels.Dashboard;
using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Application.Interfaces;

/// <summary>
/// Define o contrato para as queries de agregação do dashboard.
/// Implementado por: DashboardService no projeto Application.
/// Utilizado por: DashboardController no projeto Web.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Retorna os dados agregados para o dashboard principal.
    /// Aplica filtros opcionais por cidade e nível de risco.
    /// </summary>
    Task<DashboardViewModel> GetDashboardDataAsync(
        string? cityFilter,
        RiskRating? riskRatingFilter);
}