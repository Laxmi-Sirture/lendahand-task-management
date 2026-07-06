using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LendAHand.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;
        private readonly IFileService _fileService;


        public TasksController(
            ITaskService taskService,
            IFileService fileService,
            ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _fileService = fileService;

            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("Getting tasks for user: {UserId}", userId);
            var result = await _taskService.GetAllAsync(userId, role);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Getting task: {Id}", id);
            var result = await _taskService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateTaskDTO dto)
        {
            var createdBy = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _logger.LogInformation("Creating task: {Title}", dto.Title);
            var result = await _taskService.CreateAsync(dto, createdBy);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id, [FromBody] UpdateTaskDTO dto)
        {
            _logger.LogInformation("Updating task: {Id}", id);
            var result = await _taskService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting task: {Id}", id);
            await _taskService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/files")]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var result = await _fileService.UploadFileAsync(file, id);
            return Ok(result);
        }

        [HttpGet("{id}/files")]
        public async Task<IActionResult> GetFiles(Guid id)
        {
            var result = await _fileService.GetTaskFilesAsync(id);
            return Ok(result);
        }

        [HttpDelete("files/{fileId}")]
        public async Task<IActionResult> DeleteFile(Guid fileId)
        {
            await _fileService.DeleteFileAsync(fileId);
            return NoContent();
        }
    }
}
