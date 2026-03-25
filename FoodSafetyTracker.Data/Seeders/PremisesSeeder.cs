using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoodSafetyTracker.Data.Seeders;

/// <summary>
/// Responsável por popular o banco com 12 estabelecimentos comerciais
/// distribuídos entre as cidades Dublin, Cork e Galway com RiskRatings variados.
/// Verifica se os dados já existem antes de inserir para evitar duplicatas.
/// Chamado por: Program.cs durante a inicialização da aplicação.
/// </summary>
public class PremisesSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PremisesSeeder> _logger;

    /// <summary>
    /// Recebe o ApplicationDbContext e o ILogger via Dependency Injection.
    /// </summary>
    public PremisesSeeder(ApplicationDbContext dbContext, ILogger<PremisesSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Verifica se já existem Premises no banco antes de inserir.
    /// Se já existirem dados não faz nada para evitar duplicatas.
    /// </summary>
    public async Task SeedAsync()
    {
        if (_dbContext.Premises.Any())
        {
            return;
        }

        var premisesList = new List<Premises>
        {
            // Dublin 4 estabelecimentos
            new Premises { Name = "The Spire Café",        StreetName = "1 O'Connell Street",   City = "Dublin", RiskRating = RiskRating.High },
            new Premises { Name = "Liffey Kitchen",        StreetName = "14 Bachelors Walk",     City = "Dublin", RiskRating = RiskRating.Medium },
            new Premises { Name = "Temple Bar Grill",      StreetName = "7 Temple Bar",          City = "Dublin", RiskRating = RiskRating.High },
            new Premises { Name = "Grafton Street Deli",   StreetName = "22 Grafton Street",     City = "Dublin", RiskRating = RiskRating.Low },

            // Cork 4 estabelecimentos
            new Premises { Name = "English Market Bistro", StreetName = "Grand Parade",          City = "Cork",   RiskRating = RiskRating.Medium },
            new Premises { Name = "Leeside Bakery",        StreetName = "5 Patrick Street",      City = "Cork",   RiskRating = RiskRating.Low },
            new Premises { Name = "Shandon Steakhouse",    StreetName = "3 Shandon Street",      City = "Cork",   RiskRating = RiskRating.High },
            new Premises { Name = "Blackpool Kitchen",     StreetName = "10 Blackpool Road",     City = "Cork",   RiskRating = RiskRating.Medium },

            // Galway 4 estabelecimentos
            new Premises { Name = "Eyre Square Eats",      StreetName = "1 Eyre Square",         City = "Galway", RiskRating = RiskRating.Low },
            new Premises { Name = "Salthill Seafood",      StreetName = "8 Salthill Promenade",  City = "Galway", RiskRating = RiskRating.High },
            new Premises { Name = "Claddagh Coffee",       StreetName = "3 Claddagh Quay",       City = "Galway", RiskRating = RiskRating.Medium },
            new Premises { Name = "Shop Street Snacks",    StreetName = "15 Shop Street",        City = "Galway", RiskRating = RiskRating.Low }
        };

        _dbContext.Premises.AddRange(premisesList);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Seeded {PremisesCount} premises across Dublin, Cork and Galway", premisesList.Count);
    }
}