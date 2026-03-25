/// <summary>
/// Ponto de entrada da aplicação FoodSafetyTracker.
/// Responsável por configurar e registrar todos os serviços,
/// middlewares e dependências da aplicação.
/// A ordem de registro dos middlewares é importante e não deve ser alterada.
/// </summary>
using FoodSafetyTracker.Data;
using FoodSafetyTracker.Data.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lê a string de conexão do appsettings.json
// Em Development sobrescreve automaticamente com appsettings.Development.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o DbContext com MySQL usando o Pomelo
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registra o ASP.NET Identity com as configurações de senha e login
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // Regras de complexidade de senha
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        // Não exige confirmação de email para login em desenvolvimento
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Registra o IdentitySeeder para ser injetado via Dependency Injection
builder.Services.AddScoped<IdentitySeeder>();

// Registra os serviços do MVC e das páginas Razor do Identity
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var application = builder.Build();

// Executa os seeders antes de iniciar o pipeline HTTP
// O escopo garante que os serviços são descartados corretamente após o seed
using (var serviceScope = application.Services.CreateScope())
{
    var identitySeeder = serviceScope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await identitySeeder.SeedAsync();
}

// Configura o pipeline de requisições HTTP
if (!application.Environment.IsDevelopment())
{
    application.UseExceptionHandler("/Home/Error");
    application.UseHsts();
}

application.UseHttpsRedirection();
application.UseStaticFiles();
application.UseRouting();

// Authentication deve vir obrigatoriamente antes de Authorization
application.UseAuthentication();
application.UseAuthorization();

application.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Necessário para as páginas Razor do Identity (Login, Register, etc.)
application.MapRazorPages();

application.Run();