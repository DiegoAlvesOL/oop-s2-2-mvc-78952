
using FoodSafetyTracker.Application.Interfaces;
using FoodSafetyTracker.Application.ViewModels.Inspection;
using FoodSafetyTracker.Data.Constants;
using FoodSafetyTracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodSafetyTracker.Web.Controllers;

/// <summary>
/// Controller responsável pelo CRUD de inspeções sanitárias.
/// Criação restrita a Admin e Inspector.
/// Leitura disponível para todos os usuários autenticados.
/// Utiliza: IInspectionService e IPremisesService.
/// </summary>
[Authorize]
public class InspectionController : Controller
{
    private readonly IInspectionService _inspectionService;
    private readonly IPremisesService _premisesService;
    private readonly ILogger<InspectionController> _logger;

    /// <summary>
    /// Recebe os Services e o ILogger via Dependency Injection.
    /// </summary>
    public InspectionController(
        IInspectionService inspectionService,
        IPremisesService premisesService,
        ILogger<InspectionController> logger)
    {
        _inspectionService = inspectionService;
        _premisesService = premisesService;
        _logger = logger;
    }

    /// <summary>
    /// Exibe a lista de todas as inspeções ordenadas por data decrescente.
    /// Acessível por todos os usuários autenticados.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var allInspections = await _inspectionService.GetAllInspectionsAsync();
        return View(allInspections);
    }

    /// <summary>
    /// Exibe os detalhes de uma inspeção específica.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var inspection = await _inspectionService.GetInspectionByIdAsync(id);

        if (inspection == null)
        {
            return NotFound();
        }

        return View(inspection);
    }

    /// <summary>
    /// Exibe o formulário de criação de inspeção.
    /// Restrito a Admin e Inspector.
    /// Pré-popula o dropdown de Premises e a data de hoje.
    /// </summary>
    [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Inspector}")]
    public async Task<IActionResult> Create()
    {
        var allPremises = await _premisesService.GetAllPremisesAsync();

        var viewModel = new InspectionFormViewModel
        {
            InspectionDate = DateTime.Today,
            PremisesList = new SelectList(allPremises, "Id", "Name")
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processa o formulário de criação de inspeção.
    /// Restrito a Admin e Inspector.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Inspector}")]
    public async Task<IActionResult> Create(InspectionFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var allPremises = await _premisesService.GetAllPremisesAsync();
            viewModel.PremisesList = new SelectList(allPremises, "Id", "Name");
            return View(viewModel);
        }

        var inspection = new Inspection
        {
            PremisesId = viewModel.PremisesId,
            InspectionDate = viewModel.InspectionDate,
            Score = viewModel.Score,
            Outcome = viewModel.Outcome,
            Notes = viewModel.Notes
        };

        await _inspectionService.CreateInspectionAsync(inspection);
        TempData["SuccessMessage"] = "Inspection created successfully.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Exibe o formulário de edição de uma inspeção.
    /// Restrito ao Admin.
    /// </summary>
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        var inspection = await _inspectionService.GetInspectionByIdAsync(id);

        if (inspection == null)
        {
            return NotFound();
        }

        var allPremises = await _premisesService.GetAllPremisesAsync();

        var viewModel = new InspectionFormViewModel
        {
            Id = inspection.Id,
            PremisesId = inspection.PremisesId,
            InspectionDate = inspection.InspectionDate,
            Score = inspection.Score,
            Outcome = inspection.Outcome,
            Notes = inspection.Notes,
            PremisesList = new SelectList(allPremises, "Id", "Name", inspection.PremisesId)
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processa o formulário de edição de uma inspeção.
    /// Restrito ao Admin.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Edit(int id, InspectionFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var allPremises = await _premisesService.GetAllPremisesAsync();
            viewModel.PremisesList = new SelectList(allPremises, "Id", "Name", viewModel.PremisesId);
            return View(viewModel);
        }

        var inspection = new Inspection
        {
            Id = id,
            PremisesId = viewModel.PremisesId,
            InspectionDate = viewModel.InspectionDate,
            Score = viewModel.Score,
            Outcome = viewModel.Outcome,
            Notes = viewModel.Notes
        };

        await _inspectionService.UpdateInspectionAsync(inspection);
        TempData["SuccessMessage"] = "Inspection updated successfully.";

        return RedirectToAction(nameof(Index));
    }
}