using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LendAHand.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/employees")]
    [Authorize(Roles = "Admin")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService employeeService,
            ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true)
        {
            _logger.LogInformation("Getting all employees");
            var result = await _employeeService.GetAllAsync(
                page, pageSize, search, sortBy, ascending);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Getting employee: {Id}", id);
            var result = await _employeeService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDTO dto)
        {
            _logger.LogInformation("Creating employee: {Email}", dto.Email);
            var result = await _employeeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id, [FromBody] UpdateEmployeeDTO dto)
        {
            _logger.LogInformation("Updating employee: {Id}", id);
            var result = await _employeeService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting employee: {Id}", id);
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
