using FoodSafetyTracker.Data.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FoodSafetyTracker.Data.Seeders;

/// <summary>
/// Responsável por criar as roles e o usuário Admin inicial no banco de dados.
/// Executado uma única vez na inicialização da aplicação via Program.cs.
/// Verifica se os dados já existem antes de inserir para evitar duplicatas.
/// Chamado por: Program.cs durante a inicialização da aplicação.
/// </summary>
public class IdentitySeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentitySeeder> _logger;

    /// <summary>
    /// Recebe via Dependency Injection os serviços necessários para
    /// criar roles e usuários no banco de dados do Identity.
    /// O IConfiguration lê os valores de AdminUser:Email e AdminUser:Password
    /// do appsettings.json automaticamente.
    /// </summary>
    public IdentitySeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager,
        IConfiguration configuration,
        ILogger<IdentitySeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Ponto de entrada do seeder. Chama os métodos de criação
    /// de roles e do usuário Admin na ordem correta.
    /// As roles precisam existir antes do usuário ser criado.
    /// </summary>
    public async Task SeedAsync()
    {
        await CreateRolesAsync();
        await CreateAdminUserAsync();
    }

    /// <summary>
    /// Cria as 3 roles da aplicação: Admin, Inspector e Viewer.
    /// Verifica se cada role já existe antes de tentar criar
    /// para evitar erros em execuções subsequentes.
    /// </summary>
    private async Task CreateRolesAsync()
    {
        var requiredRoles = new[]
        {
            ApplicationRoles.Admin,
            ApplicationRoles.Inspector,
            ApplicationRoles.Viewer
        };

        foreach (var roleName in requiredRoles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation("Role {RoleName} created successfully", roleName);
            }
        }
    }

    /// <summary>
    /// Cria o usuário Admin inicial da aplicação.
    /// As credenciais (email e senha) são lidas do appsettings.json
    /// através do IConfiguration usando as chaves AdminUser:Email
    /// e AdminUser:Password. Atribui a role Admin ao usuário criado.
    /// </summary>
    private async Task CreateAdminUserAsync()
    {
        // Lê as credenciais do appsettings.json
        var adminEmail = _configuration["AdminUser:Email"];
        var adminPassword = _configuration["AdminUser:Password"];

        // Verifica se o Admin já existe para evitar duplicatas
        var existingAdminUser = await _userManager.FindByEmailAsync(adminEmail!);

        if (existingAdminUser != null)
        {
            return;
        }

        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        // Cria o usuário com a senha definida no appsettings.json
        var creationResult = await _userManager.CreateAsync(adminUser, adminPassword!);

        if (!creationResult.Succeeded)
        {
            foreach (var error in creationResult.Errors)
            {
                _logger.LogError("Failed to create Admin user: {ErrorDescription}", error.Description);
            }
            return;
        }

        // Atribui a role Admin ao usuário recém criado
        await _userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
        _logger.LogInformation("Admin user {AdminEmail} created and assigned to Admin role", adminEmail);
    }
}