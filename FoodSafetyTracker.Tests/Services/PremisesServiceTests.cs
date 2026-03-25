using FoodSafetyTracker.Application.Services;
using FoodSafetyTracker.Data;
using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace FoodSafetyTracker.Tests.Services;

/// <summary>
/// Testa o comportamento do PremisesService usando banco em memória.
/// Verifica que as operações de criação e busca funcionam corretamente.
/// Chamado por: dotnet test
/// </summary>
public class PremisesServiceTests
{
    /// <summary>
    /// Aqui eu crio um ApplicationDbContext com banco em memória e
    /// um PremisesService configurado para os testes.
    /// </summary>
    private (ApplicationDbContext dbContext, PremisesService service) CreateServiceWithInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ApplicationDbContext(options);

        // NullLogger não faz nada. Isso evita dependência de Serilog nos testes
        var logger = new NullLogger<PremisesService>();

        var service = new PremisesService(dbContext, logger);

        return (dbContext, service);
    }

    [Fact]
    public async Task CreatePremisesAsync_WhenPremisesIsValid_ShouldSaveToDatabase()
    {
        
        var (dbContext, service) = CreateServiceWithInMemoryDb();

        var premises = new Premises
        {
            Name = "Test Restaurant",
            StreetName = "123 Main Street",
            City = "Dublin",
            RiskRating = RiskRating.Medium
        };

        
        await service.CreatePremisesAsync(premises);

        
        var savedPremises = await dbContext.Premises.FindAsync(premises.Id);
        Assert.NotNull(savedPremises);
        Assert.Equal("Test Restaurant", savedPremises.Name);
    }

    [Fact]
    public async Task GetAllPremisesAsync_WhenPremisesExist_ShouldReturnAllPremises()
    {
        
        var (dbContext, service) = CreateServiceWithInMemoryDb();

        dbContext.Premises.AddRange(
            new Premises { Name = "Restaurant A", StreetName = "1 Street", City = "Dublin", RiskRating = RiskRating.Low },
            new Premises { Name = "Restaurant B", StreetName = "2 Street", City = "Cork", RiskRating = RiskRating.High }
        );
        await dbContext.SaveChangesAsync();

        
        var allPremises = await service.GetAllPremisesAsync();

        
        Assert.Equal(2, allPremises.Count);
    }

    [Fact]
    public async Task GetPremisesByIdAsync_WhenPremisesExists_ShouldReturnCorrectPremises()
    {
        
        var (dbContext, service) = CreateServiceWithInMemoryDb();

        var premises = new Premises
        {
            Name = "Test Restaurant",
            StreetName = "123 Main Street",
            City = "Dublin",
            RiskRating = RiskRating.Medium
        };

        dbContext.Premises.Add(premises);
        await dbContext.SaveChangesAsync();

        
        var foundPremises = await service.GetPremisesByIdAsync(premises.Id);
        
        Assert.NotNull(foundPremises);
        Assert.Equal("Test Restaurant", foundPremises.Name);
    }
}