using System.ComponentModel.DataAnnotations;
using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Domain.Entities;

/// <summary>
/// Representa um estabelecimento comercial sujeito a inspeções sanitárias.
/// É a entidade raiz da hierarquia: Premises -> Inspection -> FollowUp.
/// Esta entidade pertence ao Domain e não depende de nenhuma camada externa.
/// </summary>
public class Premises
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "StreetName is required.")]
    [MaxLength(300, ErrorMessage = "StreetName cannot exceed 300 characters.")]
    public string StreetName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "City is required.")]
    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
    public string City  { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Risk rating is required.")]
    public RiskRating RiskRating { get; set; }
    
    /// <summary>
    /// Navigation property, lista de todas as inspeções realizadas neste estabelecimento.
    /// O EF Core usa esta propriedade para montar o JOIN entre Premises e Inspection.
    /// </summary>
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}

