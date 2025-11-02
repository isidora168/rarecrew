using Employees.Services;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Controllers;

public class EmployeesController: Controller
{
    private readonly IEntryDataService _svc;

    public EmployeesController(IEntryDataService svc) => _svc = svc;

    public async Task<IActionResult> Index()
    {
        var totals = await _svc.GetTotalsAsync();
        return View(totals);
    }
}
