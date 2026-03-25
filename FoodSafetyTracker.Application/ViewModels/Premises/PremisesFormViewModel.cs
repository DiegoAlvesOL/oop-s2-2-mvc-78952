using System.ComponentModel.DataAnnotations;
using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Application.ViewModels.Premises;


/// <summary>
/// ViewModel para os formulários de criação e edição de estabelecimentos.
/// Contém os campos necessários para a tela e as validações de UI.
/// Utilizado por: PremisesController, Views/Premises/Create e Edit.
/// </summary>
public class PremisesFormViewModel
{
    /// <summary>
    /// Identificador do estabelecimento.
    /// Zero indica criação, valor positivo indica edição.
    /// </summary>
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    [Display(Name = "Establishment Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street name is required.")]
    [MaxLength(300, ErrorMessage = "Street name cannot exceed 300 characters.")]
    [Display(Name = "Street Address")]
    public string StreetName { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Risk rating is required.")]
    [Display(Name = "Risk Rating")]
    public RiskRating RiskRating { get; set; }
}