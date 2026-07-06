using LendAHand.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LendAHand.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/files")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IFileService fileService,
            ILogger<FilesController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("upload/{taskId}")]
        public async Task<IActionResult> Upload(
            Guid taskId, IFormFile file)
        {
            _logger.LogInformation("Uploading file for task: {TaskId}", taskId);
            var result = await _fileService.UploadFileAsync(file, taskId);
            return Ok(result);
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskFiles(Guid taskId)
        {
            var result = await _fileService.GetTaskFilesAsync(taskId);
            return Ok(result);
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> Delete(Guid fileId)
        {
            await _fileService.DeleteFileAsync(fileId);
            return NoContent();
        }
    }
}
