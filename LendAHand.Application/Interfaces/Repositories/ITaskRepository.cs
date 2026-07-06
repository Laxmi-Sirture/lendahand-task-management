using LendAHand.Domain.Entities;
using LendAHand.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Repositories
{
    public interface ITaskRepository : IGenericRepository<TaskItem>
    {
        Task<TaskItem?> GetTaskWithDetailsAsync(Guid id);
        Task<IEnumerable<TaskItem>> GetTasksByEmployeeAsync(Guid employeeId);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksDueTomorrowAsync();
        Task<IEnumerable<TaskItem>> GetByStatusesAsync(params TaskItemStatus[] statuses);   // ← naya

    }
}
