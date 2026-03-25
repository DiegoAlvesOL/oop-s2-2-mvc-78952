using FoodSafetyTracker.Application.Interfaces;
using FoodSafetyTracker.Application.ViewModels.Dashboard;
using FoodSafetyTracker.Data;
using FoodSafetyTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _dbContext;

    public DashboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Retorna os dados agregados para o dashboard principal.
    /// Constrói queries dinâmicas aplicando os filtros opcionais
    /// de cidade e nível de risco quando fornecidos.
    /// </summary>
    public async Task<DashboardViewModel> GetDashboardDataAsync(
        string? cityFilter,
        RiskRating? riskRatingFilter)
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        // Query base de inspeções — aplica os filtros condicionalmente
        // O IQueryable permite adicionar cláusulas Where sem executar a query ainda
        var inspectionsQuery = _dbContext.Inspections
            .Include(inspection => inspection.Premises)
            .AsQueryable();

        if (!string.IsNullOrEmpty(cityFilter))
        {
            inspectionsQuery = inspectionsQuery
                .Where(inspection => inspection.Premises.City == cityFilter);
        }

        if (riskRatingFilter.HasValue)
        {
            inspectionsQuery = inspectionsQuery
                .Where(inspection => inspection.Premises.RiskRating == riskRatingFilter.Value);
        }

        // Conta inspeções do mês atual aplicando os filtros
        var inspectionsThisMonth = await inspectionsQuery
            .Where(inspection => inspection.InspectionDate.Month == currentMonth
                              && inspection.InspectionDate.Year == currentYear)
            .CountAsync();

        // Conta inspeções reprovadas do mês atual aplicando os filtros
        var failedInspectionsThisMonth = await inspectionsQuery
            .Where(inspection => inspection.InspectionDate.Month == currentMonth
                              && inspection.InspectionDate.Year == currentYear
                              && inspection.Outcome == InspectionOutcome.Fail)
            .CountAsync();

        // Query base de FollowUps — aplica os filtros condicionalmente
        var followUpsQuery = _dbContext.FollowUps
            .Include(followUp => followUp.Inspection)
            .ThenInclude(inspection => inspection.Premises)
            .AsQueryable();

        if (!string.IsNullOrEmpty(cityFilter))
        {
            followUpsQuery = followUpsQuery
                .Where(followUp => followUp.Inspection.Premises.City == cityFilter);
        }

        if (riskRatingFilter.HasValue)
        {
            followUpsQuery = followUpsQuery
                .Where(followUp => followUp.Inspection.Premises.RiskRating == riskRatingFilter.Value);
        }

        // Conta FollowUps overdue: prazo vencido e ainda abertos
        var overdueFollowUps = await followUpsQuery
            .Where(followUp => followUp.DueDate < DateTime.Today
                            && followUp.Status == FollowUpStatus.Open)
            .CountAsync();

        // Busca todas as cidades disponíveis para o dropdown de filtro
        var availableCities = await _dbContext.Premises
            .AsNoTracking()
            .Select(premises => premises.City)
            .Distinct()
            .OrderBy(city => city)
            .ToListAsync();

        return new DashboardViewModel
        {
            InspectionsThisMonth = inspectionsThisMonth,
            FailedInspectionsThisMonth = failedInspectionsThisMonth,
            OverdueFollowUps = overdueFollowUps,
            CityFilter = cityFilter,
            RiskRatingFilter = riskRatingFilter,
            AvailableCities = availableCities
        };
    }
    
}