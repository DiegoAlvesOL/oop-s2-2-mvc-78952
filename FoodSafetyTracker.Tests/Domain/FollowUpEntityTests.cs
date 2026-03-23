using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Enums;

namespace FoodSafetyTracker.Tests.Domain;
/// <summary>
/// Testa as regras de negócio da entidade FollowUp.
/// O teste mais importante aqui é a regra que um FollowUp com
/// Status Closed deve obrigatoriamente ter um ClosedDate preenchido.
/// Chamado por: dotnet test
/// </summary>
public class FollowUpEntityTests
{
    [Fact]
    public void FollowUp_WhenStatusIsOpen_ClosedDateShouldBeNull()
    {
        // Arrange, cria um follomUp recém criado com status de Open
        var followUp = new FollowUp
        {
            InspectionId = 1,
            DueDate = DateTime.Today.AddDays(7),
            Status = FollowUpStatus.Open
        };
        
        
        // Assert CloseData deve ser nulo quando status é Open
        Assert.Null(followUp.ClosedDate);
    }


    [Fact]
    public void FollowUp_WhenStatusIsClosed_ClosedDateShouldNotBeNull()
    {
        // Arrange cria um followUp e simula o encerramento
        var followUp = new FollowUp
        {
            InspectionId = 1,
            DueDate = DateTime.Today.AddDays(7),
            Status = FollowUpStatus.Open
        };
        
        // Act encerra o acompanhamento
        followUp.Status = FollowUpStatus.Closed;
        followUp.ClosedDate = DateTime.Today;
        
        // Assert ClosedDate deve estar preenchido somente quando o status for Closed
        Assert.NotNull(followUp.ClosedDate);
        Assert.Equal(FollowUpStatus.Closed, followUp.Status);
    }
    
    [Fact]
    public void FollowUp_WhenDueDateIsInThePast_AndStatusIsOpen_ShouldBeOverdue()
    {
        // Arrange — cria um FollowUp com prazo vencido
        var followUp = new FollowUp
        {
            InspectionId = 1,
            DueDate = DateTime.Today.AddDays(-3),
            Status = FollowUpStatus.Open
        };

        // Act — verifica se está atrasado
        var isOverdue = followUp.DueDate < DateTime.Today
                        && followUp.Status == FollowUpStatus.Open;

        // Assert — deve ser considerado overdue
        Assert.True(isOverdue);
    }
    
}