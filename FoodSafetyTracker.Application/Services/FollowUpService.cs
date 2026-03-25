using FoodSafetyTracker.Application.Interfaces;
using FoodSafetyTracker.Data;
using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodSafetyTracker.Application.Services;

/// <summary>
/// Implementa as operações de negócio relacionadas aos acompanhamentos.
/// Responsável por criar e fechar acompanhamentos aplicando as regras de negócio.
/// Regra principal: DueDate não pode ser anterior à InspectionDate.
/// Acessa o banco via ApplicationDbContext e loga eventos importantes.
/// Implementa: IFollowUpService.
/// Chamado por: FollowUpController no projeto Web.
/// </summary>
public class FollowUpService : IFollowUpService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FollowUpService> _logger;

    /// <summary>
    /// Recebe o ApplicationDbContext e o ILogger via Dependency Injection.
    /// </summary>
    public FollowUpService(ApplicationDbContext dbContext, ILogger<FollowUpService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os acompanhamentos de uma inspeção específica
    /// incluindo os dados da inspeção relacionada.
    /// </summary>
    public async Task<List<FollowUp>> GetFollowUpsByInspectionAsync(int inspectionId)
    {
        return await _dbContext.FollowUps
            .AsNoTracking()
            .Include(followUp => followUp.Inspection)
            .Where(followUp => followUp.InspectionId == inspectionId)
            .OrderBy(followUp => followUp.DueDate)
            .ToListAsync();
    }

    /// <summary>
    /// Cria um novo acompanhamento aplicando a regra de negócio do DueDate.
    /// Loga Warning e lança InvalidOperationException se DueDate
    /// for anterior à InspectionDate da inspeção relacionada.
    /// Loga Information após salvar com sucesso.
    /// </summary>
    public async Task CreateFollowUpAsync(FollowUp followUp)
    {
        // Busca a inspeção relacionada para validar o DueDate
        var relatedInspection = await _dbContext.Inspections
            .FirstOrDefaultAsync(inspection => inspection.Id == followUp.InspectionId);

        if (relatedInspection == null)
        {
            throw new InvalidOperationException(
                $"Inspection {followUp.InspectionId} not found.");
        }

        // Regra de negócio: DueDate não pode ser anterior à InspectionDate
        if (followUp.DueDate < relatedInspection.InspectionDate)
        {
            _logger.LogWarning(
                "FollowUp DueDate {DueDate} is before InspectionDate {InspectionDate} for Inspection {InspectionId}",
                followUp.DueDate,
                relatedInspection.InspectionDate,
                followUp.InspectionId);

            throw new InvalidOperationException(
                $"Due date {followUp.DueDate:dd/MM/yyyy} cannot be before the inspection date {relatedInspection.InspectionDate:dd/MM/yyyy}.");
        }

        _dbContext.FollowUps.Add(followUp);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "FollowUp {FollowUpId} created for Inspection {InspectionId} with DueDate {DueDate}",
            followUp.Id,
            followUp.InspectionId,
            followUp.DueDate);
    }

    /// <summary>
    /// Encerra um acompanhamento preenchendo o ClosedDate e
    /// alterando o Status para Closed.
    /// Lança InvalidOperationException se o acompanhamento não for encontrado.
    /// Loga Information após encerrar com sucesso.
    /// </summary>
    public async Task CloseFollowUpAsync(int followUpId, DateTime closedDate)
    {
        var followUp = await _dbContext.FollowUps
            .FirstOrDefaultAsync(followUp => followUp.Id == followUpId);

        if (followUp == null)
        {
            throw new InvalidOperationException(
                $"FollowUp {followUpId} not found.");
        }

        followUp.Status = FollowUpStatus.Closed;
        followUp.ClosedDate = closedDate;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "FollowUp {FollowUpId} closed on {ClosedDate} for Inspection {InspectionId}",
            followUp.Id,
            closedDate,
            followUp.InspectionId);
    }
}