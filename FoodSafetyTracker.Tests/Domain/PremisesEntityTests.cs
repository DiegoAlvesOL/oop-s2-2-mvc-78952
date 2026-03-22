using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Tests.Domain;


/// <summary>
/// Testa a estrutura e comportamento básico da entidade Premises.
/// Verifica que os valores padrão estão corretos e que o relacionamento
/// com Inspection funciona conforme esperado.
/// Chamado por: dotnet test
/// </summary>
public class PremisesEntityTests
{
    [Fact]
    public void Premises_WhenCreated_ShouldHaveEmptyInspectionsList()
    {
        // Arrange cria um Premises com dados válidos
        var premises = new Premises
        {
            Name = "Teste Restaurante",
            StreetName = "219C, tonlegee road",
            City = "Dublin",
            RiskRating = RiskRating.Medium
        };
        
        
        // Assert a lista de inspeções deve existir e estar vazia
        Assert.NotNull(premises.Inspections);
        Assert.Empty(premises.Inspections);
    }

    [Fact]
    public void Premises_WhenInspectionIsAdded_ShouldContainOneInspection()
    {
        // Arrange cria um Premises com dados válidos
        var premises = new Premises
        {
            Name = "Teste Restaurante",
            StreetName = "219C, tonlegee road",
            City = "Dublin",
            RiskRating = RiskRating.High
        };

        var inspection = new Inspection
        {
            InspectionDate =  DateTime.Today,
            Score = 85,
            Outcome = InspectionOutcome.Pass,
        };
        
        // Act — adiciona a inspeção ao estabelecimento
        premises.Inspections.Add(inspection);
        
        // Assert — a lista deve conter exatamente 1 inspeção
        Assert.Single(premises.Inspections);
    }
}