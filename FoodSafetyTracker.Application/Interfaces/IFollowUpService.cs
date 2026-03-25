using FoodSafetyTracker.Domain.Entities;

namespace FoodSafetyTracker.Application.Interfaces;
/// <summary>
/// Define o contrato para operações relacionadas aos acompanhamentos.
/// Implementado por: FollowUpService no projeto Application.
/// Utilizado por: FollowUpController no projeto Web.
/// </summary>
public interface IFollowUpService
{ 
    Task<List<FollowUp>> GetFollowUpsByInspectionAsync(int inspectionId);
    Task CreateFollowUpAsync(FollowUp followUp);
    Task CloseFollowUpAsync(int followUpId, DateTime closedDate);
}