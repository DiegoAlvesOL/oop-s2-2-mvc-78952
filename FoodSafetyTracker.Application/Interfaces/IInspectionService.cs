using FoodSafetyTracker.Domain.Entities;

namespace FoodSafetyTracker.Application.Interfaces;

/// <summary>
/// Define o contrato para operações relacionadas às inspeções sanitárias.
/// Implementado por: InspectionService no projeto Application.
/// Utilizado por: InspectionController no projeto Web.
/// </summary>
public interface IInspectionService
{
    Task<List<Inspection>> GetAllInspectionsAsync();
    Task<Inspection?> GetInspectionByIdAsync(int inspectionId);
    Task<List<Inspection>> GetInspectionsByPremisesAsync(int premisesId);
    Task CreateInspectionAsync(Inspection inspection);
    Task UpdateInspectionAsync(Inspection inspection);
}