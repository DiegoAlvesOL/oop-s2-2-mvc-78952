
using FoodSafetyTracker.Application.Services;
using FoodSafetyTracker.Data;
using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Tests.Dashboard;

/// <summary>
/// Testa as queries de agregação do DashboardService usando banco em memória.
/// Verifica que as contagens e filtros retornam os valores corretos
/// com base em dados de teste conhecidos.
/// Chamado por: dotnet test
/// </summary>
public class DashboardQueryTests
{
    /// <summary>
    /// Cria um ApplicationDbContext com banco em memória e
    /// um DashboardService configurado para os testes.
    /// </summary>
    private (ApplicationDbContext dbContext, DashboardService service) CreateServiceWithInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ApplicationDbContext(options);
        var service = new DashboardService(dbContext);

        return (dbContext, service);
    }

    /// <summary>
    /// Popula o banco em memória com dados de teste conhecidos
    /// para verificar as queries do dashboard.
    /// </summary>
    private async Task SeedTestDataAsync(ApplicationDbContext dbContext)
    {
        var premisesDublin = new Premises
        {
            Name = "Dublin Restaurant",
            StreetName = "1 Main Street",
            City = "Dublin",
            RiskRating = RiskRating.High
        };

        var premisesCork = new Premises
        {
            Name = "Cork Restaurant",
            StreetName = "2 Main Street",
            City = "Cork",
            RiskRating = RiskRating.Low
        };

        dbContext.Premises.AddRange(premisesDublin, premisesCork);
        await dbContext.SaveChangesAsync();

        // Inspeção reprovada este mês em Dublin
        var inspectionFailedThisMonth = new Inspection
        {
            PremisesId = premisesDublin.Id,
            InspectionDate = DateTime.Now,
            Score = 40,
            Outcome = InspectionOutcome.Fail
        };

        // Inspeção aprovada este mês em Dublin
        var inspectionPassedThisMonth = new Inspection
        {
            PremisesId = premisesDublin.Id,
            InspectionDate = DateTime.Now,
            Score = 90,
            Outcome = InspectionOutcome.Pass
        };

        // Inspeção do mês passado em Cork
        var inspectionLastMonth = new Inspection
        {
            PremisesId = premisesCork.Id,
            InspectionDate = DateTime.Now.AddMonths(-1),
            Score = 70,
            Outcome = InspectionOutcome.Pass
        };

        dbContext.Inspections.AddRange(
            inspectionFailedThisMonth,
            inspectionPassedThisMonth,
            inspectionLastMonth);
        await dbContext.SaveChangesAsync();

        // FollowUp overdue — prazo vencido e ainda aberto
        var overdueFollowUp = new FollowUp
        {
            InspectionId = inspectionFailedThisMonth.Id,
            DueDate = DateTime.Today.AddDays(-5),
            Status = FollowUpStatus.Open
        };

        // FollowUp dentro do prazo
        var activeFollowUp = new FollowUp
        {
            InspectionId = inspectionFailedThisMonth.Id,
            DueDate = DateTime.Today.AddDays(10),
            Status = FollowUpStatus.Open
        };

        dbContext.FollowUps.AddRange(overdueFollowUp, activeFollowUp);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetDashboardDataAsync_WithNoFilters_ShouldReturnCorrectCounts()
    {
        // Arrange
        var (dbContext, service) = CreateServiceWithInMemoryDb();
        await SeedTestDataAsync(dbContext);

        // Act
        var dashboardData = await service.GetDashboardDataAsync(
            cityFilter: null,
            riskRatingFilter: null);

        // Assert — 2 inspeções este mês, 1 reprovada, 1 overdue
        Assert.Equal(2, dashboardData.InspectionsThisMonth);
        Assert.Equal(1, dashboardData.FailedInspectionsThisMonth);
        Assert.Equal(1, dashboardData.OverdueFollowUps);
    }

    [Fact]
    public async Task GetDashboardDataAsync_WithCityFilter_ShouldReturnOnlyCityData()
    {
        // Arrange
        var (dbContext, service) = CreateServiceWithInMemoryDb();
        await SeedTestDataAsync(dbContext);

        // Act — filtra apenas Cork
        var dashboardData = await service.GetDashboardDataAsync(
            cityFilter: "Cork",
            riskRatingFilter: null);

        // Assert — Cork não tem inspeções este mês
        Assert.Equal(0, dashboardData.InspectionsThisMonth);
        Assert.Equal(0, dashboardData.FailedInspectionsThisMonth);
    }

    [Fact]
    public async Task GetDashboardDataAsync_ShouldReturnAvailableCities()
    {
        // Arrange
        var (dbContext, service) = CreateServiceWithInMemoryDb();
        await SeedTestDataAsync(dbContext);

        // Act
        var dashboardData = await service.GetDashboardDataAsync(
            cityFilter: null,
            riskRatingFilter: null);

        // Assert — deve retornar as 2 cidades disponíveis
        Assert.Equal(2, dashboardData.AvailableCities.Count);
        Assert.Contains("Dublin", dashboardData.AvailableCities);
        Assert.Contains("Cork", dashboardData.AvailableCities);
    }
}