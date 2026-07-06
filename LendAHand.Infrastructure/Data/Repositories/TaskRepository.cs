using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Domain.Entities;
using LendAHand.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Infrastructure.Data.Repositories
{
    public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TaskItem?> GetTaskWithDetailsAsync(Guid id)
            => await _context.Tasks
                .Include(t => t.AssignedEmployee)
                .Include(t => t.FileUploads)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<TaskItem>> GetTasksByEmployeeAsync(
            Guid employeeId)
            => await _context.Tasks
                .Include(t => t.AssignedEmployee)
                .AsNoTracking()
                .Where(t => t.AssignedEmployeeId == employeeId)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
            => await _context.Tasks
                .Include(t => t.AssignedEmployee)
                .AsNoTracking()
                .Where(t => t.Status != TaskItemStatus.Completed
                    && t.DueDate < DateTime.UtcNow)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetTasksDueTomorrowAsync()
            => await _context.Tasks
                .Include(t => t.AssignedEmployee)
                .AsNoTracking()
                .Where(t => t.Status != TaskItemStatus.Completed
                    && t.DueDate.Date == DateTime.UtcNow.AddDays(1).Date)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetByStatusesAsync(params TaskItemStatus[] statuses)
            => await _context.Tasks
                .Include(t => t.AssignedEmployee)
                .AsNoTracking()
                .Where(t => statuses.Contains(t.Status))
                .ToListAsync();
    }

}
