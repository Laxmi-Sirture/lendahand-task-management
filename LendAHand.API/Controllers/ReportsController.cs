using LendAHand.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LendAHand.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            IReportService reportService,
            ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("completed-tasks")]
        public async Task<IActionResult> GetCompletedTasks()
        {
            var result = await _reportService.GetCompletedTasksReportAsync();
            return Ok(result);
        }

        [HttpGet("pending-tasks")]
        public async Task<IActionResult> GetPendingTasks()
        {
            var result = await _reportService.GetPendingTasksReportAsync();
            return Ok(result);
        }

        [HttpGet("employee-wise")]
        public async Task<IActionResult> GetEmployeeWise()
        {
            var result = await _reportService.GetEmployeeWiseReportAsync();
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(
            [FromQuery] string type,
            [FromQuery] string reportType)
        {
            if (type == "excel")
            {
                var bytes = await _reportService.ExportToExcelAsync(reportType);
                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{reportType}_report.xlsx");
            }
            else if (type == "csv")
            {
                var bytes = await _reportService.ExportToCsvAsync(reportType);
                return File(bytes, "text/csv", $"{reportType}_report.csv");
            }

            return BadRequest(new { message = "Invalid export type" });
        }
    }
}
