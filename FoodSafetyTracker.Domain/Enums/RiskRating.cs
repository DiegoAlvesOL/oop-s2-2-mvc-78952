namespace FoodSafetyTracker.Domain.Enums;

/// <summary>
/// Representa o nível de risco sanitário de um estabelecimento comercial.
/// Utilizado para priorizar a frequência das inspeções.
/// Utilizado por: Premises, DashboardService.
/// Este enum pertence ao Domain e não depende de nenhuma camada externa.
/// </summary>
public enum RiskRating
{
    Low = 1,
    Medium = 2,
    High = 3
}