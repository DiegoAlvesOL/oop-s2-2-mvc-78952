
using FoodSafetyTracker.Application.Interfaces;
using FoodSafetyTracker.Data;
using FoodSafetyTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodSafetyTracker.Application.Services;

/// <summary>
/// Implementa as operações de negócio relacionadas aos estabelecimentos comerciais.
/// Responsável por criar, atualizar, buscar e deletar estabelecimentos.
/// Acessa o banco via ApplicationDbContext e loga eventos importantes.
/// Implementa: IPremisesService.
/// Chamado por: PremisesController no projeto Web.
/// </summary>
public class PremisesService : IPremisesService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PremisesService> _logger;

    /// <summary>
    /// Recebe o ApplicationDbContext e o ILogger via Dependency Injection.
    /// </summary>
    public PremisesService(ApplicationDbContext dbContext, ILogger<PremisesService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os estabelecimentos ordenados por nome.
    /// Usa AsNoTracking pois é uma operação de leitura pura.
    /// </summary>
    public async Task<List<Premises>> GetAllPremisesAsync()
    {
        return await _dbContext.Premises
            .AsNoTracking()
            .OrderBy(premises => premises.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna um estabelecimento pelo Id.
    /// Retorna null se não encontrar.
    /// </summary>
    public async Task<Premises?> GetPremisesByIdAsync(int premisesId)
    {
        return await _dbContext.Premises
            .AsNoTracking()
            .FirstOrDefaultAsync(premises => premises.Id == premisesId);
    }

    /// <summary>
    /// Cria um novo estabelecimento no banco de dados.
    /// Loga Information após salvar com sucesso.
    /// </summary>
    public async Task CreatePremisesAsync(Premises premises)
    {
        _dbContext.Premises.Add(premises);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Premises {PremisesId} {PremisesName} created in {City}",
            premises.Id,
            premises.Name,
            premises.City);
    }

    /// <summary>
    /// Atualiza os dados de um estabelecimento existente no banco de dados.
    /// Loga Information após salvar com sucesso.
    /// </summary>
    public async Task UpdatePremisesAsync(Premises premises)
    {
        _dbContext.Premises.Update(premises);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Premises {PremisesId} {PremisesName} updated",
            premises.Id,
            premises.Name);
    }

    /// <summary>
    /// Remove um estabelecimento do banco de dados pelo Id.
    /// Não loga pois a operação de delete não é auditada neste sistema.
    /// </summary>
    public async Task DeletePremisesAsync(int premisesId)
    {
        var premises = await _dbContext.Premises.FindAsync(premisesId);

        if (premises == null)
        {
            return;
        }

        _dbContext.Premises.Remove(premises);
        await _dbContext.SaveChangesAsync();
    }
}