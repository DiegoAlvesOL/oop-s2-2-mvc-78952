
using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoodSafetyTracker.Data.Seeders;

/// <summary>
/// Responsável por popular o banco com 25 inspeções distribuídas
/// entre os 12 estabelecimentos com datas e resultados variados.
/// Garante inspeções no mês atual para o dashboard mostrar dados relevantes.
/// Verifica se os dados já existem antes de inserir para evitar duplicatas.
/// Chamado por: Program.cs durante a inicialização da aplicação.
/// </summary>
public class InspectionSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<InspectionSeeder> _logger;

    /// <summary>
    /// Recebe o ApplicationDbContext e o ILogger via Dependency Injection.
    /// </summary>
    public InspectionSeeder(ApplicationDbContext dbContext, ILogger<InspectionSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Verifica se já existem Inspections no banco antes de inserir.
    /// Depende dos Premises já terem sido criados pelo PremisesSeeder.
    /// </summary>
    public async Task SeedAsync()
    {
        if (_dbContext.Inspections.Any())
        {
            return;
        }

        // Busca os IDs dos Premises criados pelo PremisesSeeder
        var premisesIds = _dbContext.Premises
            .OrderBy(premises => premises.Id)
            .Select(premises => premises.Id)
            .ToList();

        if (premisesIds.Count < 12)
        {
            _logger.LogWarning("Cannot seed Inspections — expected 12 Premises but found {Count}", premisesIds.Count);
            return;
        }

        var currentMonth = DateTime.Now;
        var lastMonth = DateTime.Now.AddMonths(-1);
        var twoMonthsAgo = DateTime.Now.AddMonths(-2);

        var inspectionsList = new List<Inspection>
        {
            // Inspeções este mês — essenciais para o dashboard
            new Inspection { PremisesId = premisesIds[0],  InspectionDate = currentMonth.AddDays(-2),  Score = 45, Outcome = InspectionOutcome.Fail, Notes = "Poor hygiene in kitchen area" },
            new Inspection { PremisesId = premisesIds[1],  InspectionDate = currentMonth.AddDays(-3),  Score = 88, Outcome = InspectionOutcome.Pass, Notes = "All areas clean and compliant" },
            new Inspection { PremisesId = premisesIds[2],  InspectionDate = currentMonth.AddDays(-5),  Score = 52, Outcome = InspectionOutcome.Fail, Notes = "Temperature controls not adequate" },
            new Inspection { PremisesId = premisesIds[3],  InspectionDate = currentMonth.AddDays(-7),  Score = 91, Outcome = InspectionOutcome.Pass, Notes = "Excellent food storage practices" },
            new Inspection { PremisesId = premisesIds[4],  InspectionDate = currentMonth.AddDays(-8),  Score = 60, Outcome = InspectionOutcome.Fail, Notes = "Pest control issues identified" },
            new Inspection { PremisesId = premisesIds[5],  InspectionDate = currentMonth.AddDays(-10), Score = 78, Outcome = InspectionOutcome.Pass, Notes = "Minor labelling issues corrected on site" },
            new Inspection { PremisesId = premisesIds[6],  InspectionDate = currentMonth.AddDays(-12), Score = 55, Outcome = InspectionOutcome.Fail, Notes = "Staff hygiene training required" },
            new Inspection { PremisesId = premisesIds[7],  InspectionDate = currentMonth.AddDays(-14), Score = 83, Outcome = InspectionOutcome.Pass, Notes = "Good overall compliance" },

            // Inspeções mês passado
            new Inspection { PremisesId = premisesIds[8],  InspectionDate = lastMonth.AddDays(-2),  Score = 47, Outcome = InspectionOutcome.Fail, Notes = "Cross contamination risk observed" },
            new Inspection { PremisesId = premisesIds[9],  InspectionDate = lastMonth.AddDays(-4),  Score = 92, Outcome = InspectionOutcome.Pass, Notes = "Outstanding hygiene standards" },
            new Inspection { PremisesId = premisesIds[10], InspectionDate = lastMonth.AddDays(-6),  Score = 71, Outcome = InspectionOutcome.Pass, Notes = "Satisfactory conditions" },
            new Inspection { PremisesId = premisesIds[11], InspectionDate = lastMonth.AddDays(-8),  Score = 58, Outcome = InspectionOutcome.Fail, Notes = "Waste disposal not compliant" },
            new Inspection { PremisesId = premisesIds[0],  InspectionDate = lastMonth.AddDays(-10), Score = 75, Outcome = InspectionOutcome.Pass, Notes = "Improvement noted since last visit" },
            new Inspection { PremisesId = premisesIds[1],  InspectionDate = lastMonth.AddDays(-12), Score = 86, Outcome = InspectionOutcome.Pass, Notes = "All requirements met" },
            new Inspection { PremisesId = premisesIds[2],  InspectionDate = lastMonth.AddDays(-15), Score = 43, Outcome = InspectionOutcome.Fail, Notes = "Repeat violation — cooling equipment" },
            new Inspection { PremisesId = premisesIds[3],  InspectionDate = lastMonth.AddDays(-18), Score = 95, Outcome = InspectionOutcome.Pass, Notes = "Exemplary food safety standards" },

            // Inspeções dois meses atrás
            new Inspection { PremisesId = premisesIds[4],  InspectionDate = twoMonthsAgo.AddDays(-3),  Score = 62, Outcome = InspectionOutcome.Pass, Notes = "Pest issue resolved" },
            new Inspection { PremisesId = premisesIds[5],  InspectionDate = twoMonthsAgo.AddDays(-5),  Score = 50, Outcome = InspectionOutcome.Fail, Notes = "Inadequate hand washing facilities" },
            new Inspection { PremisesId = premisesIds[6],  InspectionDate = twoMonthsAgo.AddDays(-7),  Score = 79, Outcome = InspectionOutcome.Pass, Notes = "Staff training completed" },
            new Inspection { PremisesId = premisesIds[7],  InspectionDate = twoMonthsAgo.AddDays(-9),  Score = 88, Outcome = InspectionOutcome.Pass, Notes = "Well maintained premises" },
            new Inspection { PremisesId = premisesIds[8],  InspectionDate = twoMonthsAgo.AddDays(-11), Score = 44, Outcome = InspectionOutcome.Fail, Notes = "Food storage temperatures exceeded" },
            new Inspection { PremisesId = premisesIds[9],  InspectionDate = twoMonthsAgo.AddDays(-13), Score = 81, Outcome = InspectionOutcome.Pass, Notes = "Good hygiene practices observed" },
            new Inspection { PremisesId = premisesIds[10], InspectionDate = twoMonthsAgo.AddDays(-15), Score = 67, Outcome = InspectionOutcome.Pass, Notes = "Acceptable standards maintained" },
            new Inspection { PremisesId = premisesIds[11], InspectionDate = twoMonthsAgo.AddDays(-17), Score = 53, Outcome = InspectionOutcome.Fail, Notes = "Cleaning schedule not followed" },
            new Inspection { PremisesId = premisesIds[0],  InspectionDate = twoMonthsAgo.AddDays(-20), Score = 76, Outcome = InspectionOutcome.Pass, Notes = "Significant improvement observed" }
        };

        _dbContext.Inspections.AddRange(inspectionsList);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Seeded {InspectionCount} inspections across 3 months", inspectionsList.Count);
    }
}