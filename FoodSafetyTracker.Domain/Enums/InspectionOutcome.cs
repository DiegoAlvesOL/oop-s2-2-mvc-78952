namespace FoodSafetyTracker.Domain.Enums;

/// <summary>
/// Representa o resultado de uma inspeção realizada em um estabelecimento comercial.
/// Pass indica que o estabelecimento está em conformidade.
/// Fail indica que foram encontradas irregularidades que exigem acompanhamento.
/// Utilizado por: Inspection, InspectionService, DashboardService.
/// Este enum pertence ao Domain e não depende de nenhuma camada externa.
/// </summary>
public enum InspectionOutcome
{
    Pass = 1,
    Fail = 2,
}