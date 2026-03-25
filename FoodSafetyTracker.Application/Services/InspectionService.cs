
using FoodSafetyTracker.Application.Interfaces;
using FoodSafetyTracker.Data;
using FoodSafetyTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodSafetyTracker.Application.Services;

/// <summary>
/// Implementa as operações de negócio relacionadas às inspeções sanitárias.
/// Responsável por criar, atualizar e buscar inspeções.
/// Acessa o banco via ApplicationDbContext e loga eventos importantes.
/// Implementa: IInspectionService.
/// Chamado por: InspectionController no projeto Web.
/// </summary>
public class InspectionService : IInspectionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<InspectionService> _logger;

    /// <summary>
    /// Recebe o ApplicationDbContext e o ILogger via Dependency Injection.
    /// </summary>
    public InspectionService(ApplicationDbContext dbContext, ILogger<InspectionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as inspeções incluindo os dados do estabelecimento relacionado.
    /// Ordenadas por data decrescente — mais recentes primeiro.
    /// Usa AsNoTracking pois é uma operação de leitura pura.
    /// </summary>
    public async Task<List<Inspection>> GetAllInspectionsAsync()
    {
        return await _dbContext.Inspections
            .AsNoTracking()
            .Include(inspection => inspection.Premises)
            .OrderByDescending(inspection => inspection.InspectionDate)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna uma inspeção pelo Id incluindo os dados do estabelecimento.
    /// Retorna null se não encontrar.
    /// </summary>
    public async Task<Inspection?> GetInspectionByIdAsync(int inspectionId)
    {
        return await _dbContext.Inspections
            .AsNoTracking()
            .Include(inspection => inspection.Premises)
            .FirstOrDefaultAsync(inspection => inspection.Id == inspectionId);
    }

    /// <summary>
    /// Retorna todas as inspeções de um estabelecimento específico
    /// ordenadas por data decrescente.
    /// </summary>
    public async Task<List<Inspection>> GetInspectionsByPremisesAsync(int premisesId)
    {
        return await _dbContext.Inspections
            .AsNoTracking()
            .Where(inspection => inspection.PremisesId == premisesId)
            .OrderByDescending(inspection => inspection.InspectionDate)
            .ToListAsync();
    }

    /// <summary>
    /// Cria uma nova inspeção no banco de dados.
    /// Loga Warning se o Score for zero — pode indicar erro de preenchimento.
    /// Loga Information após salvar com sucesso.
    /// </summary>
    public async Task CreateInspectionAsync(Inspection inspection)
    {
        if (inspection.Score == 0)
        {
            _logger.LogWarning(
                "Inspection Score is zero for Premises {PremisesId} — possible data entry error",
                inspection.PremisesId);
        }

        _dbContext.Inspections.Add(inspection);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Inspection {InspectionId} created for Premises {PremisesId} with Outcome {Outcome}",
            inspection.Id,
            inspection.PremisesId,
            inspection.Outcome);
    }

    /// <summary>
    /// Atualiza os dados de uma inspeção existente no banco de dados.
    /// </summary>
    public async Task UpdateInspectionAsync(Inspection inspection)
    {
        _dbContext.Inspections.Update(inspection);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Inspection {InspectionId} updated for Premises {PremisesId}",
            inspection.Id,
            inspection.PremisesId);
    }
}