
using FoodSafetyTracker.Application.Interfaces;
using FoodSafetyTracker.Application.ViewModels.FollowUp;
using FoodSafetyTracker.Data.Constants;
using FoodSafetyTracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafetyTracker.Web.Controllers;

/// <summary>
/// Controller responsável pela criação e encerramento de acompanhamentos.
/// Criação e encerramento restritos a Admin e Inspector.
/// Leitura disponível para todos os usuários autenticados.
/// Utiliza: IFollowUpService e IInspectionService.
/// </summary>
[Authorize]
public class FollowUpController : Controller
{
    private readonly IFollowUpService _followUpService;
    private readonly IInspectionService _inspectionService;
    private readonly ILogger<FollowUpController> _logger;

    /// <summary>
    /// Recebe os Services e o ILogger via Dependency Injection.
    /// </summary>
    public FollowUpController(
        IFollowUpService followUpService,
        IInspectionService inspectionService,
        ILogger<FollowUpController> logger)
    {
        _followUpService = followUpService;
        _inspectionService = inspectionService;
        _logger = logger;
    }

    /// <summary>
    /// Exibe a lista de acompanhamentos de uma inspeção específica.
    /// Acessível por todos os usuários autenticados.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var allFollowUps = await _followUpService.GetAllFollowUpsAsync();
        return View(allFollowUps);
    }

    /// <summary>
    /// Exibe os acompanhamentos de uma inspeção específica.
    /// Usada pelo botão na tela de Details da Inspection.
    /// Acessível por todos os usuários autenticados.
    /// </summary>
    public async Task<IActionResult> ByInspection(int inspectionId)
    {
        var inspection = await _inspectionService.GetInspectionByIdAsync(inspectionId);

        if (inspection == null)
        {
            return NotFound();
        }

        var followUps = await _followUpService.GetFollowUpsByInspectionAsync(inspectionId);
        ViewBag.Inspection = inspection;

        return View("ByInspection", followUps);
    }

    /// <summary>
    /// Exibe o formulário de criação de acompanhamento.
    /// Restrito a Admin e Inspector.
    /// Pré-popula com os dados da inspeção relacionada para contexto.
    /// </summary>
    [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Inspector}")]
    public async Task<IActionResult> Create(int inspectionId)
    {
        var inspection = await _inspectionService.GetInspectionByIdAsync(inspectionId);

        if (inspection == null)
        {
            return NotFound();
        }

        var viewModel = new FollowUpFormViewModel
        {
            InspectionId = inspection.Id,
            InspectionDate = inspection.InspectionDate,
            PremisesName = inspection.Premises?.Name,
            DueDate = DateTime.Today.AddDays(14)
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processa o formulário de criação de acompanhamento.
    /// Restrito a Admin e Inspector.
    /// Exibe mensagem de erro se DueDate for anterior à InspectionDate.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Inspector}")]
    public async Task<IActionResult> Create(FollowUpFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            var followUp = new FollowUp
            {
                InspectionId = viewModel.InspectionId,
                DueDate = viewModel.DueDate,
                Status = viewModel.Status
            };

            await _followUpService.CreateFollowUpAsync(followUp);
            TempData["SuccessMessage"] = "Follow-up created successfully.";

            return RedirectToAction(nameof(Index), new { inspectionId = viewModel.InspectionId });
        }
        catch (InvalidOperationException exception)
        {
            // Exibe a mensagem de erro da regra de negócio na View
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(viewModel);
        }
    }

    /// <summary>
    /// Exibe o formulário de encerramento de um acompanhamento.
    /// Restrito a Admin e Inspector.
    /// Pré-popula o ClosedDate com a data de hoje.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Inspector}")]
    public async Task<IActionResult> Close(int id)
    {
        var followUp = await _followUpService.GetFollowUpByIdAsync(id);

        if (followUp == null)
        {
            return NotFound();
        }

        var viewModel = new FollowUpFormViewModel
        {
            Id = followUp.Id,
            InspectionId = followUp.InspectionId,
            PremisesName = followUp.Inspection?.Premises?.Name,
            InspectionDate = followUp.Inspection?.InspectionDate ?? DateTime.Today,
            DueDate = followUp.DueDate,
            Status = followUp.Status,
            ClosedDate = DateTime.Today
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processa o encerramento de um acompanhamento.
    /// Restrito a Admin e Inspector.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Inspector}")]
    public async Task<IActionResult> Close(int id, FollowUpFormViewModel viewModel)
    {
        try
        {
            await _followUpService.CloseFollowUpAsync(id, viewModel.ClosedDate ?? DateTime.Today);
            TempData["SuccessMessage"] = "Follow-up closed successfully.";

            return RedirectToAction(nameof(Index), new { inspectionId = viewModel.InspectionId });
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(viewModel);
        }
    }
}