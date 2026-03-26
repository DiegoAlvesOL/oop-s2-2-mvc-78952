/// <summary>
/// Controller responsável pelo gerenciamento de usuários do sistema.
/// Todas as operações restritas ao Admin.
/// Utiliza: UserManager e RoleManager do ASP.NET Identity.
/// </summary>
using FoodSafetyTracker.Application.ViewModels.User;
using FoodSafetyTracker.Data.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Web.Controllers;

[Authorize(Roles = ApplicationRoles.Admin)]
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Recebe o UserManager, RoleManager e ILogger via Dependency Injection.
    /// </summary>
    public UserController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os usuários do sistema com suas roles.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var allUsers = await _userManager.Users.ToListAsync();

        var userListWithRoles = new List<(IdentityUser User, string Role)>();

        foreach (var user in allUsers)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var primaryRole = userRoles.FirstOrDefault() ?? "No Role";
            userListWithRoles.Add((user, primaryRole));
        }

        return View(userListWithRoles);
    }

    /// <summary>
    /// Exibe o formulário de criação de usuário.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new UserFormViewModel());
    }

    /// <summary>
    /// Processa o formulário de criação de usuário.
    /// Cria o usuário via UserManager e atribui a role selecionada.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var newUser = new IdentityUser
        {
            UserName = viewModel.UserName,
            Email = viewModel.Email,
            EmailConfirmed = true
        };

        var creationResult = await _userManager.CreateAsync(newUser, viewModel.Password!);

        if (!creationResult.Succeeded)
        {
            foreach (var error in creationResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(viewModel);
        }

        await _userManager.AddToRoleAsync(newUser, viewModel.Role);

        _logger.LogInformation(
            "User {UserEmail} created with role {Role} by Admin",
            viewModel.Email,
            viewModel.Role);

        TempData["SuccessMessage"] = $"User '{viewModel.Email}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Exibe o formulário de edição de um usuário existente.
    /// Pré-popula com os dados atuais do usuário.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var viewModel = new UserFormViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Role = userRoles.FirstOrDefault() ?? string.Empty
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processa o formulário de edição de um usuário.
    /// Atualiza email, username, senha (se fornecida) e role.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        user.Email = viewModel.Email;
        user.UserName = viewModel.UserName;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(viewModel);
        }

        // Atualiza a senha apenas se foi fornecida
        if (!string.IsNullOrWhiteSpace(viewModel.Password))
        {
            var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, passwordToken, viewModel.Password);

            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(viewModel);
            }
        }

        // Atualiza a role removendo as antigas e adicionando a nova
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, viewModel.Role);

        _logger.LogInformation(
            "User {UserEmail} updated with role {Role} by Admin",
            viewModel.Email,
            viewModel.Role);

        TempData["SuccessMessage"] = $"User '{viewModel.Email}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Exibe a tela de confirmação de exclusão de um usuário.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var viewModel = new UserFormViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Role = userRoles.FirstOrDefault() ?? "No Role"
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processa a exclusão de um usuário.
    /// Impede que o Admin delete a si próprio.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, UserFormViewModel viewModel)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Impede que o Admin delete a si próprio
        var currentUserId = _userManager.GetUserId(User);
        if (user.Id == currentUserId)
        {
            TempData["ErrorMessage"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }

        await _userManager.DeleteAsync(user);

        _logger.LogInformation(
            "User {UserEmail} deleted by Admin",
            user.Email);

        TempData["SuccessMessage"] = $"User '{user.Email}' deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}