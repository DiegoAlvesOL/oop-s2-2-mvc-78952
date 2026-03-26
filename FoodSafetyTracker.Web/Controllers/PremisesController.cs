
using FoodSafetyTracker.Application.Interfaces;
using FoodSafetyTracker.Application.ViewModels.Premises;
using FoodSafetyTracker.Data.Constants;
using FoodSafetyTracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FoodSafetyTracker.Web.Controllers;

/// <summary>
/// Controller responsável pelo CRUD de estabelecimentos comerciais.
/// Operações de escrita restritas ao Admin.
/// Leitura disponível para todos os usuários autenticados.
/// Utiliza: IPremisesService para todas as operações de negócio.
/// </summary>
[Authorize]
public class PremisesController : Controller
{
    private readonly IPremisesService _premisesService;
    private readonly ILogger<PremisesController> _logger;

    /// <summary>
    /// Recebe o IPremisesService e o ILogger via Dependency Injection.
    /// </summary>
    public PremisesController(IPremisesService premisesService, ILogger<PremisesController> logger)
    {
        _premisesService = premisesService;
        _logger = logger;
    }

    /// <summary>
    /// Exibe a lista de todos os estabelecimentos.
    /// Acessível por todos os usuários autenticados.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var allPremises = await _premisesService.GetAllPremisesAsync();
        return View(allPremises);
    }

    /// <summary>
    /// Exibe os detalhes de um estabelecimento específico.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var premises = await _premisesService.GetPremisesByIdAsync(id);

        if (premises == null)
        {
            return NotFound();
        }

        return View(premises);
    }

    /// <summary>
    /// Exibe o formulário de criação de estabelecimento.
    /// Restrito ao Admin.
    /// </summary>
    [Authorize(Roles = ApplicationRoles.Admin)]
    public IActionResult Create()
    {
        return View(new PremisesFormViewModel());
    }

    /// <summary>
    /// Processa o formulário de criação de estabelecimento.
    /// Restrito ao Admin.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Create(PremisesFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var premises = new Premises
        {
            Name = viewModel.Name,
            StreetName = viewModel.StreetName,
            City = viewModel.City,
            RiskRating = viewModel.RiskRating
        };

        await _premisesService.CreatePremisesAsync(premises);
        TempData["SuccessMessage"] = $"Premises '{premises.Name}' created successfully.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Exibe o formulário de edição de um estabelecimento.
    /// Restrito ao Admin.
    /// </summary>
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        var premises = await _premisesService.GetPremisesByIdAsync(id);

        if (premises == null)
        {
            return NotFound();
        }

        var viewModel = new PremisesFormViewModel
        {
            Id = premises.Id,
            Name = premises.Name,
            StreetName = premises.StreetName,
            City = premises.City,
            RiskRating = premises.RiskRating
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processa o formulário de edição de um estabelecimento.
    /// Restrito ao Admin.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Edit(int id, PremisesFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var premises = new Premises
        {
            Id = id,
            Name = viewModel.Name,
            StreetName = viewModel.StreetName,
            City = viewModel.City,
            RiskRating = viewModel.RiskRating
        };

        await _premisesService.UpdatePremisesAsync(premises);
        TempData["SuccessMessage"] = $"Premises '{premises.Name}' updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Processa a exclusão de um estabelecimento.
    /// Restrito ao Admin.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _premisesService.DeletePremisesAsync(id);
        TempData["SuccessMessage"] = "Premises deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}