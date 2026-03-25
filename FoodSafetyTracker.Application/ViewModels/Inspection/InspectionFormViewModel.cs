
using System.ComponentModel.DataAnnotations;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodSafetyTracker.Application.ViewModels.Inspection;

/// <summary>
/// ViewModel para os formulários de criação e edição de inspeções.
/// Contém a lista de estabelecimentos para o dropdown e os campos da inspeção.
/// O campo InspectionDate é pré-populado com a data de hoje mas editável.
/// Utilizado por: InspectionController, Views/Inspection/Create e Edit.
/// </summary>
public class InspectionFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Premises is required.")]
    [Display(Name = "Establishment")]
    public int PremisesId { get; set; }

 
    public SelectList? PremisesList { get; set; }

    [Required(ErrorMessage = "Inspection date is required.")]
    [Display(Name = "Inspection Date")]
    public DateTime InspectionDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Score is required.")]
    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    [Display(Name = "Score (0-100)")]
    public int Score { get; set; }

    [Required(ErrorMessage = "Outcome is required.")]
    [Display(Name = "Outcome")]
    public InspectionOutcome Outcome { get; set; }

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
}