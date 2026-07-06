using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LendAHand.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            IEmployeeService employeeService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            _logger.LogInformation("Getting admin dashboard");
            var result = await _dashboardService.GetAdminDashboardAsync();
            return Ok(result);
        }

        [HttpGet("employee")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetEmployeeDashboard()
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var employee = await _employeeService.GetByUserIdAsync(userId);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            var result = await _dashboardService
                .GetEmployeeDashboardAsync(employee.Id);
            return Ok(result);
        }
    }
}
