using LendAHand.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDTO>> GetAllAsync(Guid? userId, string? role);
        Task<TaskDTO?> GetByIdAsync(Guid id);
        Task<TaskDTO> CreateAsync(CreateTaskDTO dto, Guid createdBy);
        Task<TaskDTO> UpdateAsync(Guid id, UpdateTaskDTO dto);
        Task DeleteAsync(Guid id);
    }
}
