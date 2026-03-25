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
using Serilog;

// Configura o Serilog lendo as definições do appsettings.json
// Isso deve acontecer antes de qualquer outra coisa para capturar
// erros que ocorram durante a inicialização da aplicação
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting FoodSafetyTracker application");

    var builder = WebApplication.CreateBuilder(args);

    // Substitui o sistema de logging padrão do ASP.NET Core pelo Serilog
    // Todo código que usa ILogger passa a usar o Serilog automaticamente
    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Registra o DbContext com MySQL usando o Pomelo
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    // Registra o ASP.NET Identity com as configurações de senha e login
    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
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
    application.UseAuthentication();
    application.UseAuthorization();

    application.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    application.MapRazorPages();

    application.Run();
}
catch (Exception exception)
{
    // Captura erros fatais durante a inicialização da aplicação
    // como falha de conexão com o banco ou configuração inválida
    Log.Fatal(exception, "FoodSafetyTracker application terminated unexpectedly");
}
finally
{
    // Garante que todos os logs pendentes são gravados antes de encerrar
    Log.CloseAndFlush();
}