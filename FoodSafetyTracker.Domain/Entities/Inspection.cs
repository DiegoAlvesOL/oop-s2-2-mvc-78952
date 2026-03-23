using System.ComponentModel.DataAnnotations;
using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Domain.Entities;


/// <summary>
/// epresenta uma inspeção sanitária realizada em um estabelecimento comercial.
/// Cada inspeção está vinculada a um Premises e pode gerar múltiplos FollowUps.
/// Esta entidade pertence ao Domain e não depende de nenhuma camada externa.
/// </summary>
public class Inspection
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Premises is required.")]
    public int PremisesId { get; set; }
    
    [Required(ErrorMessage = "Inspection Date is required.")]
    public DateTime InspectionDate { get; set; }
    
    [Required(ErrorMessage = "Score is required.")]
    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    public int Score { get; set; }

    [Required(ErrorMessage = "Outcome is required.")]
    public InspectionOutcome Outcome { get; set; }

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Navigation property — referência ao estabelecimento onde esta inspeção foi realizada.
    /// O EF Core usa esta propriedade para montar o JOIN entre Inspection e Premises.
    /// </summary>
    public Premises Premises { get; set; } = null!;

    /// <summary>
    /// Navigation property — lista de todos os acompanhamentos gerados por esta inspeção.
    /// O EF Core usa esta propriedade para montar o JOIN entre Inspection e FollowUp.
    /// </summary>
    public ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();
    
}