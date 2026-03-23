using System.ComponentModel.DataAnnotations;
using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Domain.Entities;

/// <summary>
/// Representa um acompanhamento gerado a partir de uma irregularidade
/// encontrada em uma inspeção reprovada.
/// O campo ClosedDate é nullable pois só é preenchido quando o status muda para Closed.
/// Utilizado por: FollowUpService, DashboardService.
/// Esta entidade pertence ao Domain e não depende de nenhuma camada externa.
/// </summary>
public class FollowUp
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Inspection is required")]
    public int InspectionId { get; set; }
    
    [Required(ErrorMessage = "Due date is required.")]
    public DateTime DueDate { get; set; }
    
    [Required(ErrorMessage = "Status is required.")]
    public FollowUpStatus Status { get; set; }
    
    public DateTime? ClosedDate { get; set; }

    public Inspection Inspection { get; set; } = null!;

}
