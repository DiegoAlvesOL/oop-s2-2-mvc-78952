using System.ComponentModel.DataAnnotations;
using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Application.ViewModels.FollowUp;

/// <summary>
/// ViewModel para o formulário de criação de acompanhamentos.
/// O DueDate é obrigatório — o fiscal define o prazo de correção.
/// O ClosedDate é pré-populado com hoje ao fechar mas editável.
/// Utilizado por: FollowUpController, Views/FollowUp/Create e Close.
/// </summary>
public class FollowUpFormViewModel
{
    
    public int Id { get; set; }

    [Required(ErrorMessage = "Inspection is required.")]
    [Display(Name = "Inspection")]
    public int InspectionId { get; set; }

    
    [Display(Name = "Establishment")]
    public string? PremisesName { get; set; }

    
    [Display(Name = "Inspection Date")]
    public DateTime InspectionDate { get; set; }

    [Required(ErrorMessage = "Due date is required.")]
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; }

    [Display(Name = "Status")]
    public FollowUpStatus Status { get; set; } = FollowUpStatus.Open;

    /// <summary>
    /// Data de encerramento do acompanhamento.
    /// Pré-populado com hoje ao fechar mas editável pelo fiscal.
    /// Null quando o acompanhamento ainda está aberto.
    /// </summary>
    [Display(Name = "Closed Date")]
    public DateTime? ClosedDate { get; set; }
}