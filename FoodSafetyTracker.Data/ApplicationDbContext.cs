/// <summary>
/// Representa a sessão com o banco de dados da aplicação.
/// Herda de IdentityDbContext para incluir automaticamente as tabelas
/// do ASP.NET Identity junto com as tabelas do domínio.
/// Chamado por: PremisesService, InspectionService, FollowUpService, DashboardService.
/// Registrado no Program.cs via Dependency Injection.
/// </summary>
using FoodSafetyTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Representa a tabela de estabelecimentos comerciais no banco de dados.
    /// </summary>
    public DbSet<Premises> Premises { get; set; }

    /// <summary>
    /// Representa a tabela de inspeções no banco de dados.
    /// </summary>
    public DbSet<Inspection> Inspections { get; set; }

    /// <summary>
    /// Representa a tabela de acompanhamentos no banco de dados.
    /// </summary>
    public DbSet<FollowUp> FollowUps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Necessário chamar o base para que o Identity configure
        base.OnModelCreating(modelBuilder);

        // Configura o relacionamento Premises -> Inspections
        // Um Premises pode ter muitas Inspections
        // Se um Premises for deletado, suas Inspections também são deletadas
        modelBuilder.Entity<Premises>()
            .HasMany(premises => premises.Inspections)
            .WithOne(inspection => inspection.Premises)
            .HasForeignKey(inspection => inspection.PremisesId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configura o relacionamento Inspection -> FollowUps
        // Uma Inspection pode ter muitos FollowUps
        // Se uma Inspection for deletada, seus FollowUps também são deletados
        modelBuilder.Entity<Inspection>()
            .HasMany(inspection => inspection.FollowUps)
            .WithOne(followUp => followUp.Inspection)
            .HasForeignKey(followUp => followUp.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}