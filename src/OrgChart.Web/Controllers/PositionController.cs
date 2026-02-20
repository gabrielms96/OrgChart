using Microsoft.AspNetCore.Mvc;
using OrgChart.Application.DTOs;
using OrgChart.Application.Services;
using OrgChart.Application.Validators;
using OrgChart.Domain.Enums;

namespace OrgChart.Web.Controllers;

public class PositionController : Controller
{
    private readonly IPositionService _service;
    private readonly PositionCreateDtoValidator _createValidator;
    private readonly PositionUpdateDtoValidator _updateValidator;

    public PositionController(
        IPositionService service,
        PositionCreateDtoValidator createValidator,
        PositionUpdateDtoValidator updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _service.GetAllAsync();
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(new List<PositionDto>());
        }

        return View(result.Data);
    }

    public IActionResult Create()
    {
        ViewBag.Levels = GetLevels();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PositionCreateDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            ViewBag.Levels = GetLevels();
            return View(dto);
        }

        var result = await _service.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            ViewBag.Levels = GetLevels();
            return View(dto);
        }

        TempData["Success"] = "Cargo criado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var dto = new PositionUpdateDto
        {
            Id = result.Data!.Id,
            Name = result.Data.Name,
            Level = result.Data.Level,
            IsActive = result.Data.IsActive
        };

        ViewBag.Levels = GetLevels();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PositionUpdateDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            ViewBag.Levels = GetLevels();
            return View(dto);
        }

        var result = await _service.UpdateAsync(dto);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            ViewBag.Levels = GetLevels();
            return View(dto);
        }

        TempData["Success"] = "Cargo atualizado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
        }
        else
        {
            TempData["Success"] = "Cargo excluído com sucesso!";
        }

        return RedirectToAction(nameof(Index));
    }

    private Dictionary<int, string> GetLevels()
    {
        return new Dictionary<int, string>
        {
            { (int)EPositionLevel.Intern, "Estagiário" },
            { (int)EPositionLevel.Junior, "Júnior" },
            { (int)EPositionLevel.MidLevel, "Pleno" },
            { (int)EPositionLevel.Senior, "Sênior" },
            { (int)EPositionLevel.Coordinator, "Coordenação" },
            { (int)EPositionLevel.Manager, "Gerência" },
            { (int)EPositionLevel.Director, "Diretoria" }
        };
    }
}
