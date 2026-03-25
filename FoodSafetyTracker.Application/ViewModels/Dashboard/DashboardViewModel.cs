using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Application.ViewModels.Dashboard;

/// <summary>
/// Representa os dados agregados exibidos na página do Dashboard.
/// Contém as contagens de inspeções e acompanhamentos do mês atual
/// além dos filtros selecionados pelo usuário.
/// Utilizado por: DashboardController, IDashboardService.
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// Total de inspeções realizadas no mês atual.
    /// </summary>
    public int InspectionsThisMonth { get; set; }

    /// <summary>
    /// Total de inspeções reprovadas no mês atual.
    /// </summary>
    public int FailedInspectionsThisMonth { get; set; }

    /// <summary>
    /// Total de acompanhamentos com prazo vencido e status ainda Open.
    /// </summary>
    public int OverdueFollowUps { get; set; }

    /// <summary>
    /// Filtro de cidade selecionado pelo usuário. Null significa sem filtro.
    /// </summary>
    public string? CityFilter { get; set; }

    /// <summary>
    /// Filtro de nível de risco selecionado pelo usuário. Null significa sem filtro.
    /// </summary>
    public RiskRating? RiskRatingFilter { get; set; }

    /// <summary>
    /// Lista de cidades disponíveis para o dropdown de filtro.
    /// </summary>
    public List<string> AvailableCities { get; set; } = new List<string>();
}