/// <summary>
/// Define o contrato para operações relacionadas aos acompanhamentos.
/// Implementado por: FollowUpService no projeto Application.
/// Utilizado por: FollowUpController no projeto Web.
/// </summary>
using FoodSafetyTracker.Domain.Entities;

namespace FoodSafetyTracker.Application.Interfaces;

public interface IFollowUpService
{
    Task<List<FollowUp>> GetAllFollowUpsAsync();
    Task<List<FollowUp>> GetFollowUpsByInspectionAsync(int inspectionId);
    Task<FollowUp?> GetFollowUpByIdAsync(int followUpId);
    Task CreateFollowUpAsync(FollowUp followUp);
    Task CloseFollowUpAsync(int followUpId, DateTime closedDate);
}