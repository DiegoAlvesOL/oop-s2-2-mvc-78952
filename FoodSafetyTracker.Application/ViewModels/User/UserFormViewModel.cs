using System.ComponentModel.DataAnnotations;
using FoodSafetyTracker.Data.Constants;

namespace FoodSafetyTracker.Application.ViewModels.User;

/// <summary>
/// ViewModel para os formulários de criação e edição de usuários.
/// Utilizado por: UserController, Views/User/Create e Edit.
/// </summary>
public class UserFormViewModel
{
    /// <summary>
    /// Identificador do usuário.
    /// Vazio indica criação, preenchido indica edição.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Senha obrigatória apenas na criação.
    /// Na edição deixar em branco mantém a senha atual.
    /// </summary>
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Lista de roles disponíveis para o dropdown.
    /// Populada pelo Controller antes de renderizar a View.
    /// </summary>
    public List<string> AvailableRoles { get; set; } = new List<string>
    {
        ApplicationRoles.Admin,
        ApplicationRoles.Inspector,
        ApplicationRoles.Viewer
    };
}