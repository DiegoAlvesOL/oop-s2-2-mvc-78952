using FoodSafetyTracker.Data;
using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Tests.Seeders;

/// <summary>
/// Testa que o ApplicationDbContext é criado corretamente
/// usando um banco de dados em memória.
/// O banco em memória é usado nos testes para não depender
/// de uma instância real do MySQL — isso garante que os testes
/// rodam em qualquer máquina inclusive no CI do GitHub Actions.
/// Chamado por: dotnet test
/// </summary>
public class DbContextTests
{
    private ApplicationDbContext CreateInMemoryDbContext()
    {
        // Cria as opções do DbContext apontando para um banco em memória
        // Cada teste recebe um banco com nome único para não interferir nos outros
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task DbContext_WhenPremisesIsSaved_ShouldBeRetrievable()
    {
        // Arrange — cria o banco em memória e um Premises
        await using var dbContext = CreateInMemoryDbContext();

        var premises = new Premises
        {
            Name = "Test Restaurant",
            StreetName = "123 Main Street",
            City = "Dublin",
            RiskRating = RiskRating.Medium
        };

        // Act — salva o Premises no banco em memória
        dbContext.Premises.Add(premises);
        await dbContext.SaveChangesAsync();

        // Assert — verifica que o Premises foi salvo e pode ser recuperado
        var savedPremises = await dbContext.Premises.FindAsync(premises.Id);
        Assert.NotNull(savedPremises);
        Assert.Equal("Test Restaurant", savedPremises.Name);
    }
    
}