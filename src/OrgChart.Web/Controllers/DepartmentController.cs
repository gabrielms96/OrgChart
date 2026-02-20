using Microsoft.AspNetCore.Mvc;
using OrgChart.Application.DTOs;
using OrgChart.Application.Services;
using OrgChart.Application.Validators;

namespace OrgChart.Web.Controllers;

public class DepartmentController : Controller
{
    private readonly IDepartmentService _service;
    private readonly DepartmentCreateDtoValidator _createValidator;
    private readonly DepartmentUpdateDtoValidator _updateValidator;

    public DepartmentController(
        IDepartmentService service,
        DepartmentCreateDtoValidator createValidator,
        DepartmentUpdateDtoValidator updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        var result = string.IsNullOrWhiteSpace(searchTerm)
            ? await _service.GetAllAsync()
            : await _service.SearchByNameAsync(searchTerm);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(new List<DepartmentDto>());
        }

        ViewBag.SearchTerm = searchTerm;
        return View(result.Data);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartmentCreateDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(dto);
        }

        var result = await _service.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(dto);
        }

        TempData["Success"] = "Departamento criado com sucesso!";
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

        var dto = new DepartmentUpdateDto
        {
            Id = result.Data!.Id,
            Name = result.Data.Name,
            Code = result.Data.Code,
            IsActive = result.Data.IsActive
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DepartmentUpdateDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(dto);
        }

        var result = await _service.UpdateAsync(dto);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(dto);
        }

        TempData["Success"] = "Departamento atualizado com sucesso!";
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
            TempData["Success"] = "Departamento excluído com sucesso!";
        }

        return RedirectToAction(nameof(Index));
    }
}
