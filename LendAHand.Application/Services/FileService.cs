using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Entities;
using LendAHand.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace LendAHand.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _uploadPath;

        public FileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        public async Task<FileUploadDTO> UploadFileAsync(IFormFile file, Guid taskId)
        {
            if (file == null || file.Length == 0)
                throw new ValidationException("No file was provided");

            if (file.Length > 5 * 1024 * 1024)
                throw new ValidationException("File size cannot exceed 5MB");

            var allowedTypes = new List<string> { ".pdf", ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedTypes.Contains(ext))
                throw new ValidationException("Only PDF, JPG, PNG files are allowed");

            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException(nameof(TaskItem), taskId);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUpload = new FileUpload
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = file.FileName,
                FilePath = filePath,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.FileUploads.AddAsync(fileUpload);
            await _unitOfWork.CompleteAsync();

            return new FileUploadDTO
            {
                Id = fileUpload.Id,
                FileName = fileUpload.FileName,
                FilePath = fileUpload.FilePath,
                FileSize = fileUpload.FileSize,
                CreatedAt = fileUpload.CreatedAt
            };
        }

        public async Task<IEnumerable<FileUploadDTO>> GetTaskFilesAsync(Guid taskId)
        {
            var files = await _unitOfWork.FileUploads.GetByTaskIdAsync(taskId);
            return files.Select(f => new FileUploadDTO
            {
                Id = f.Id,
                FileName = f.FileName,
                FilePath = f.FilePath,
                FileSize = f.FileSize,
                CreatedAt = f.CreatedAt
            });
        }

        public async Task DeleteFileAsync(Guid fileId)
        {
            var file = await _unitOfWork.FileUploads.GetByIdAsync(fileId);
            if (file == null)
                throw new NotFoundException(nameof(FileUpload), fileId);

            if (File.Exists(file.FilePath))
                File.Delete(file.FilePath);

            _unitOfWork.FileUploads.Remove(file);
            await _unitOfWork.CompleteAsync();
        }
    }
}