namespace FoodSafetyTracker.Domain.Enums;


/// <summary>
/// Representa o status de um acompanhamento gerado a partir de uma inspeção reprovada.
/// Open indica que a irregularidade ainda não foi corrigida.
/// Closed indica que a irregularidade foi resolvida e o acompanhamento foi encerrado.
/// Utilizado por: FollowUp, FollowUpService, DashboardService.
/// Este enum pertence ao Domain e não depende de nenhuma camada externa.
/// </summary>
public enum FollowUpStatus
{
    Open = 1,
    Closed = 2,
}