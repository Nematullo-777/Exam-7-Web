using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class HomeController(IDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var summary = await dashboardService.GetSummaryAsync();
        var topCourses = await dashboardService.GetTopCoursesAsync();
        var byMonth = await dashboardService.GetEnrollmentsByMonthAsync();

        ViewBag.Summary = summary.Value;
        ViewBag.TopCourses = topCourses.Value;
        ViewBag.ByMonth = byMonth.Value;

        return View();
    }
}
