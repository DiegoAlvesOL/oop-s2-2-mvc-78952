using FoodSafetyTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Data;


public class ApplicationDbContext : DbContext
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
        // Configurando o relacionamento Premises -> Inspections
        // Um Premises pode ter muitas Inspections
        // Se um Premises for deletado, suas Inspections também são deletadas
        modelBuilder.Entity<Premises>()
            .HasMany(premises => premises.Inspections)
            .WithOne(inspection => inspection.Premises)
            .HasForeignKey(inspection => inspection.PremisesId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configurando o relacionamento Inspection -> FollowUps
        // Uma Inspection pode ter muitos FollowUps
        // Se uma Inspection for deletada, seus FollowUps também são deletados
        modelBuilder.Entity<Inspection>()
            .HasMany(inspection => inspection.FollowUps)
            .WithOne(followUp => followUp.Inspection)
            .HasForeignKey(followUp => followUp.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
