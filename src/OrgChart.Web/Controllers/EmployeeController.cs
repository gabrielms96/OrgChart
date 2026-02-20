using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrgChart.Application.DTOs;
using OrgChart.Application.Services;
using OrgChart.Application.Validators;

namespace OrgChart.Web.Controllers;

public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IPositionService _positionService;
    private readonly EmployeeCreateDtoValidator _createValidator;
    private readonly EmployeeUpdateDtoValidator _updateValidator;

    public EmployeeController(
        IEmployeeService employeeService,
        IDepartmentService departmentService,
        IPositionService positionService,
        EmployeeCreateDtoValidator createValidator,
        EmployeeUpdateDtoValidator updateValidator)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _positionService = positionService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _employeeService.GetAllAsync();
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(new List<EmployeeDto>());
        }

        return View(result.Data);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeCreateDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _employeeService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            await PopulateDropdownsAsync();
            return View(dto);
        }

        TempData["Success"] = "Colaborador criado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _employeeService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var dto = new EmployeeUpdateDto
        {
            Id = result.Data!.Id,
            Name = result.Data.Name,
            Email = result.Data.Email,
            DepartmentId = result.Data.DepartmentId,
            PositionId = result.Data.PositionId,
            HireDate = result.Data.HireDate,
            ManagerId = result.Data.ManagerId
        };

        await PopulateDropdownsAsync(id);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EmployeeUpdateDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            await PopulateDropdownsAsync(dto.Id);
            return View(dto);
        }

        var result = await _employeeService.UpdateAsync(dto);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            await PopulateDropdownsAsync(dto.Id);
            return View(dto);
        }

        TempData["Success"] = "Colaborador atualizado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _employeeService.GetByIdAsync(id);
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
        var result = await _employeeService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
        }
        else
        {
            TempData["Success"] = "Colaborador excluído com sucesso!";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(int? currentEmployeeId = null)
    {
        var departments = await _departmentService.GetAllAsync();
        ViewBag.Departments = new SelectList(
            departments.Data?.Where(d => d.IsActive),
            "Id",
            "Name"
        );

        var positions = await _positionService.GetAllAsync();
        ViewBag.Positions = new SelectList(
            positions.Data?.Where(p => p.IsActive),
            "Id",
            "Name"
        );

        var employees = await _employeeService.GetAllAsync();
        var employeeList = employees.Data?
            .Where(e => !currentEmployeeId.HasValue || e.Id != currentEmployeeId.Value)
            .Select(e => new { e.Id, Name = $"{e.Name} - {e.PositionName}" })
            .ToList();

        ViewBag.Managers = new SelectList(employeeList, "Id", "Name");
    }
}
