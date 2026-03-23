/// <summary>
/// Ponto de entrada da aplicação FoodSafetyTracker.
/// Responsável por configurar e registrar todos os serviços,
/// middlewares e dependências da aplicação.
/// </summary>
using FoodSafetyTracker.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lê a string de conexão do appsettings.json
// Em Development usa o appsettings.Development.json automaticamente
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o DbContext com MySQL usando o Pomelo
// ServerVersion.AutoDetect detecta automaticamente a versão do MySQL instalada
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registra os serviços do MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

var application = builder.Build();

// Configura o pipeline de requisições HTTP
if (!application.Environment.IsDevelopment())
{
    application.UseExceptionHandler("/Home/Error");
    application.UseHsts();
}

application.UseHttpsRedirection();
application.UseStaticFiles();
application.UseRouting();
application.UseAuthorization();

application.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

application.Run();