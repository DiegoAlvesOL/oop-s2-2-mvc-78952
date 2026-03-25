using FoodSafetyTracker.Data;
using FoodSafetyTracker.Data.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Tests.Authorization;

public class IdentityConfigurationTests
{
    /// <summary>
    /// Nesse primeiro passo estou criando uma instância do ApplicationDbContext apontando para um banco
    /// em memória com nome único. O nome único garante que cada teste
    /// recebe um banco limpo e isolado dos outros testes.
    /// </summary>
    /// <returns></returns>
    private ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new ApplicationDbContext(options);
    }


    [Fact]
    public async Task Identity_WhenUserIsSaved_ShouldBeRetrievable()
    {
        // Arrange neste passo está sendo criado o banco em memória e um usuário Inspector
        await using var dbContext = CreateInMemoryDbContext();

        var user = new IdentityUser
        {
            UserName = "diego_inspector@foodsafety.ie",
            Email = "diego_inspector@foodsafety.ie"
        };
        
        // Act o meu Act salva o usuário no banco em memória
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        
        //O meu Assert verifica se o usuário foi salvo e pode ser recuperado
        var savedUser = await dbContext.Users
            .FirstOrDefaultAsync(identityUser => identityUser.Email == "diego_inspector@foodsafety.ie");
        
        Assert.NotNull(savedUser);
        Assert.Equal("diego_inspector@foodsafety.ie", savedUser.Email);
    }
    
    [Fact]
    public async Task Identity_WhenRoleIsCreated_ShouldBeRetrievable()
    {
        // Arrange — cria o banco em memória e a role Admin
        await using var dbContext = CreateInMemoryDbContext();

        var role = new IdentityRole
        {
            Name = ApplicationRoles.Admin,
            NormalizedName = ApplicationRoles.Admin.ToUpper()
        };

        // Act — salva a role no banco em memória
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        // Assert — verifica que a role foi salva e pode ser recuperada
        var savedRole = await dbContext.Roles
            .FirstOrDefaultAsync(identityRole => identityRole.Name == ApplicationRoles.Admin);

        Assert.NotNull(savedRole);
        Assert.Equal(ApplicationRoles.Admin, savedRole.Name);
    }

    [Fact]
    public async Task Identity_WhenAllRolesAreCreated_ShouldHaveThreeRoles()
    {
        // Arrange — cria o banco em memória e as 3 roles da aplicação
        await using var dbContext = CreateInMemoryDbContext();

        var roles = new[]
        {
            new IdentityRole
            {
                Name = ApplicationRoles.Admin,
                NormalizedName = ApplicationRoles.Admin.ToUpper()
            },
            new IdentityRole
            {
                Name = ApplicationRoles.Inspector,
                NormalizedName = ApplicationRoles.Inspector.ToUpper()
            },
            new IdentityRole
            {
                Name = ApplicationRoles.Viewer,
                NormalizedName = ApplicationRoles.Viewer.ToUpper()
            }
        };

        // Act — salva as 3 roles no banco em memória
        dbContext.Roles.AddRange(roles);
        await dbContext.SaveChangesAsync();

        // Assert — verifica que existem exatamente 3 roles
        var totalRoles = await dbContext.Roles.CountAsync();
        Assert.Equal(3, totalRoles);
    }
    
}