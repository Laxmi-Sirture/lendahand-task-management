using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardDTO> GetAdminDashboardAsync()
        {
            var totalEmployees = await _unitOfWork.Employees
                .CountAsync();
            var totalTasks = await _unitOfWork.Tasks
                .CountAsync();
            var completedTasks = await _unitOfWork.Tasks
                .CountAsync(t => t.Status == TaskItemStatus.Completed);
            var pendingTasks = await _unitOfWork.Tasks
                .CountAsync(t => t.Status == TaskItemStatus.Pending);
            var inProgressTasks = await _unitOfWork.Tasks
                .CountAsync(t => t.Status == TaskItemStatus.InProgress);

            return new AdminDashboardDTO
            {
                TotalEmployees = totalEmployees,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                InProgressTasks = inProgressTasks
            };
        }

        public async Task<EmployeeDashboardDTO> GetEmployeeDashboardAsync(
            Guid employeeId)
        {
            var myTasks = await _unitOfWork.Tasks
                .CountAsync(t => t.AssignedEmployeeId == employeeId);
            var completedTasks = await _unitOfWork.Tasks
                .CountAsync(t => t.AssignedEmployeeId == employeeId
                    && t.Status == TaskItemStatus.Completed);
            var pendingTasks = await _unitOfWork.Tasks
                .CountAsync(t => t.AssignedEmployeeId == employeeId
                    && t.Status == TaskItemStatus.Pending);
            var overdueTasks = await _unitOfWork.Tasks
                .CountAsync(t => t.AssignedEmployeeId == employeeId
                    && t.Status != TaskItemStatus.Completed
                    && t.DueDate < DateTime.UtcNow);

            return new EmployeeDashboardDTO
            {
                MyTasks = myTasks,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                OverdueTasks = overdueTasks
            };
        }
    }
}
