
using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoodSafetyTracker.Data.Seeders;

/// <summary>
/// Responsável por popular o banco com 10 acompanhamentos com cenários variados.
/// Cobre os cenários: overdue, dentro do prazo e fechados.
/// Verifica se os dados já existem antes de inserir para evitar duplicatas.
/// Chamado por: Program.cs durante a inicialização da aplicação.
/// </summary>
public class FollowUpSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FollowUpSeeder> _logger;

    /// <summary>
    /// Recebe o ApplicationDbContext e o ILogger via Dependency Injection.
    /// </summary>
    public FollowUpSeeder(ApplicationDbContext dbContext, ILogger<FollowUpSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Verifica se já existem FollowUps no banco antes de inserir.
    /// Depende das Inspections já terem sido criadas pelo InspectionSeeder.
    /// Cria cenários variados: overdue, dentro do prazo e fechados.
    /// </summary>
    public async Task SeedAsync()
    {
        if (_dbContext.FollowUps.Any())
        {
            return;
        }

        // Busca apenas as inspeções reprovadas para criar FollowUps
        var failedInspectionIds = _dbContext.Inspections
            .Where(inspection => inspection.Outcome == InspectionOutcome.Fail)
            .OrderBy(inspection => inspection.Id)
            .Select(inspection => new { inspection.Id, inspection.InspectionDate })
            .ToList();

        if (failedInspectionIds.Count < 4)
        {
            _logger.LogWarning("Cannot seed FollowUps — expected at least 4 failed Inspections but found {Count}", failedInspectionIds.Count);
            return;
        }

        var followUpsList = new List<FollowUp>
        {
            // FollowUps overdue, prazo vencido e ainda abertos
            new FollowUp { InspectionId = failedInspectionIds[0].Id, DueDate = DateTime.Today.AddDays(-10), Status = FollowUpStatus.Open,   ClosedDate = null },
            new FollowUp { InspectionId = failedInspectionIds[0].Id, DueDate = DateTime.Today.AddDays(-5),  Status = FollowUpStatus.Open,   ClosedDate = null },
            new FollowUp { InspectionId = failedInspectionIds[1].Id, DueDate = DateTime.Today.AddDays(-3),  Status = FollowUpStatus.Open,   ClosedDate = null },

            // FollowUps dentro do prazo, abertos mas não vencidos
            new FollowUp { InspectionId = failedInspectionIds[1].Id, DueDate = DateTime.Today.AddDays(7),   Status = FollowUpStatus.Open,   ClosedDate = null },
            new FollowUp { InspectionId = failedInspectionIds[2].Id, DueDate = DateTime.Today.AddDays(14),  Status = FollowUpStatus.Open,   ClosedDate = null },
            new FollowUp { InspectionId = failedInspectionIds[2].Id, DueDate = DateTime.Today.AddDays(21),  Status = FollowUpStatus.Open,   ClosedDate = null },

            // FollowUps fechados, problema resolvido
            new FollowUp { InspectionId = failedInspectionIds[3].Id, DueDate = DateTime.Today.AddDays(-20), Status = FollowUpStatus.Closed, ClosedDate = DateTime.Today.AddDays(-15) },
            new FollowUp { InspectionId = failedInspectionIds[3].Id, DueDate = DateTime.Today.AddDays(-15), Status = FollowUpStatus.Closed, ClosedDate = DateTime.Today.AddDays(-10) },
            new FollowUp { InspectionId = failedInspectionIds[0].Id, DueDate = DateTime.Today.AddDays(-25), Status = FollowUpStatus.Closed, ClosedDate = DateTime.Today.AddDays(-20) },
            new FollowUp { InspectionId = failedInspectionIds[1].Id, DueDate = DateTime.Today.AddDays(-12), Status = FollowUpStatus.Closed, ClosedDate = DateTime.Today.AddDays(-8) }
        };

        _dbContext.FollowUps.AddRange(followUpsList);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Seeded {FollowUpCount} follow-ups — 3 overdue, 3 active, 4 closed",
            followUpsList.Count);
    }
}