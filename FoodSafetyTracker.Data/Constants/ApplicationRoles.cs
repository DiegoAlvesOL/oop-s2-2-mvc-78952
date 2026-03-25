namespace FoodSafetyTracker.Data.Constants;

/// <summary>
/// Define os nomes das roles disponíveis na aplicação como constantes.
/// Centraliza os nomes para evitar erros de digitação em todo o projeto.
/// Utilizado por: IdentitySeeder, todos os Controllers com [Authorize(Roles = ...)].
/// </summary>
public static class ApplicationRoles
{
    public const string Admin = "Admin";
    public const string Inspector = "Inspector";
    public const string Viewer = "Viewer";
}