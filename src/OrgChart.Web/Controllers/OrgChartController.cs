using Microsoft.AspNetCore.Mvc;
using OrgChart.Application.Services;

namespace OrgChart.Web.Controllers;

public class OrgChartController : Controller
{
    private readonly IOrgChartService _service;

    public OrgChartController(IOrgChartService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _service.GetOrgChartAsync();
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View();
        }

        return View(result.Data);
    }

    // API endpoint: GET /api/orgchart
    [HttpGet]
    [Route("api/orgchart")]
    public async Task<IActionResult> GetOrgChartJson()
    {
        var result = await _service.GetOrgChartAsync();
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Json(result.Data);
    }
}
