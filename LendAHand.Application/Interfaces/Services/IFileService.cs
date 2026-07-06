using LendAHand.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Services
{
    public interface IFileService
    {
        Task<FileUploadDTO> UploadFileAsync(IFormFile file, Guid taskId);
        Task<IEnumerable<FileUploadDTO>> GetTaskFilesAsync(Guid taskId);
        Task DeleteFileAsync(Guid fileId);
    }
}
