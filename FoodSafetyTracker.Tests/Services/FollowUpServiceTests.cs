using FoodSafetyTracker.Application.Services;
using FoodSafetyTracker.Data;
using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace FoodSafetyTracker.Tests.Services;

public class FollowUpServiceTests
{
    /// <summary>
    /// Cria um ApplicationDbContext com banco em memória e
    /// um FollowUpService configurado para os testes.
    /// </summary>
    private (ApplicationDbContext dbContext, FollowUpService service) CreateServiceWithInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ApplicationDbContext(options);
        var logger = new NullLogger<FollowUpService>();
        var service = new FollowUpService(dbContext, logger);

        return (dbContext, service);
    }

    [Fact]
    public async Task CreateFollowUpAsync_WhenDueDateIsBeforeInspectionDate_ShouldThrowException()
    {
        // Arrange — cria uma inspeção e um FollowUp com DueDate inválido
        var (dbContext, service) = CreateServiceWithInMemoryDb();

        var inspection = new Inspection
        {
            PremisesId = 1,
            InspectionDate = new DateTime(2026, 3, 25),
            Score = 50,
            Outcome = InspectionOutcome.Fail
        };

        dbContext.Inspections.Add(inspection);
        await dbContext.SaveChangesAsync();

        var followUp = new FollowUp
        {
            InspectionId = inspection.Id,
            DueDate = new DateTime(2026, 3, 20), // anterior à InspectionDate
            Status = FollowUpStatus.Open
        };

        // Act e Assert — verifica que a exceção é lançada
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateFollowUpAsync(followUp));
    }

    [Fact]
    public async Task CreateFollowUpAsync_WhenDueDateIsAfterInspectionDate_ShouldSaveSuccessfully()
    {
        // Arrange — cria uma inspeção e um FollowUp com DueDate válido
        var (dbContext, service) = CreateServiceWithInMemoryDb();

        var inspection = new Inspection
        {
            PremisesId = 1,
            InspectionDate = new DateTime(2026, 3, 25),
            Score = 50,
            Outcome = InspectionOutcome.Fail
        };

        dbContext.Inspections.Add(inspection);
        await dbContext.SaveChangesAsync();

        var followUp = new FollowUp
        {
            InspectionId = inspection.Id,
            DueDate = new DateTime(2026, 4, 10), // posterior à InspectionDate
            Status = FollowUpStatus.Open
        };

        // Act
        await service.CreateFollowUpAsync(followUp);

        // Assert
        var savedFollowUp = await dbContext.FollowUps.FindAsync(followUp.Id);
        Assert.NotNull(savedFollowUp);
        Assert.Equal(FollowUpStatus.Open, savedFollowUp.Status);
    }

    [Fact]
    public async Task CloseFollowUpAsync_WhenFollowUpExists_ShouldSetStatusToClosedAndFillClosedDate()
    {
        // Arrange — cria um FollowUp aberto
        var (dbContext, service) = CreateServiceWithInMemoryDb();

        var followUp = new FollowUp
        {
            InspectionId = 1,
            DueDate = new DateTime(2026, 4, 10),
            Status = FollowUpStatus.Open
        };

        dbContext.FollowUps.Add(followUp);
        await dbContext.SaveChangesAsync();

        var closedDate = new DateTime(2026, 4, 5);

        // Act
        await service.CloseFollowUpAsync(followUp.Id, closedDate);

        // Assert
        var closedFollowUp = await dbContext.FollowUps.FindAsync(followUp.Id);
        Assert.NotNull(closedFollowUp);
        Assert.Equal(FollowUpStatus.Closed, closedFollowUp.Status);
        Assert.Equal(closedDate, closedFollowUp.ClosedDate);
    }
}