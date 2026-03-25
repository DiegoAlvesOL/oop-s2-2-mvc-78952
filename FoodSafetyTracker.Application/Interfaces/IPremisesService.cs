using FoodSafetyTracker.Domain.Entities;

namespace FoodSafetyTracker.Application.Interfaces;

/// <summary>
/// Define o contrato para operações relacionadas aos estabelecimentos comerciais.
/// Implementado por: PremisesService no projeto Application.
/// Utilizado por: PremisesController no projeto Web.
/// </summary>
public interface IPremisesService
{
    
    Task<List<Premises>> GetAllPremisesAsync();
    Task<Premises?> GetPremisesByIdAsync(int premisesId);
    Task CreatePremisesAsync(Premises premises);
    Task UpdatePremisesAsync(Premises premises);    
    Task DeletePremisesAsync(int premisesId);
}