using AutoMapper;
using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Entities;
using LendAHand.Domain.Enums;
using LendAHand.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public TaskService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<TaskDTO>> GetAllAsync(
            Guid? userId, string? role)
        {
            IEnumerable<TaskItem> tasks;

            if (role == "Admin")
            {
                tasks = await _unitOfWork.Tasks.GetAllAsync();
            }
            else
            {
                var employee = await _unitOfWork.Employees
                    .GetByUserIdAsync(userId!.Value);
                if (employee == null)
                    return new List<TaskDTO>();

                tasks = await _unitOfWork.Tasks
                    .GetTasksByEmployeeAsync(employee.Id);
            }

            return _mapper.Map<IEnumerable<TaskDTO>>(tasks);
        }

        public async Task<TaskDTO?> GetByIdAsync(Guid id)
        {
            var task = await _unitOfWork.Tasks.GetTaskWithDetailsAsync(id);
            if (task == null)
                throw new NotFoundException(nameof(TaskItem), id);

            return _mapper.Map<TaskDTO>(task);
        }

        public async Task<TaskDTO> CreateAsync(
            CreateTaskDTO dto, Guid createdBy)
        {
            // Business Rule: DueDate >= StartDate
            if (dto.DueDate < dto.StartDate)
                throw new ValidationException(
                    "Due date cannot be earlier than start date");

            var employee = await _unitOfWork.Employees
                .GetByIdAsync(dto.AssignedEmployeeId);
            if (employee == null)
                throw new NotFoundException(
                    nameof(Employee), dto.AssignedEmployeeId);

            if (!Enum.TryParse<TaskPriority>(dto.Priority, out var priority))
                throw new ValidationException("Invalid priority value");

            if (!Enum.TryParse<TaskItemStatus>(dto.Status, out var status))
                throw new ValidationException("Invalid status value");

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Priority = priority,
                Status = status,
                StartDate = dto.StartDate,
                DueDate = dto.DueDate,
                AssignedEmployeeId = dto.AssignedEmployeeId,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.CompleteAsync();

            // Notification — Task Assigned
            await _notificationService.CreateNotificationAsync(
                employee.UserId,
                $"New task assigned: {task.Title}");

            return _mapper.Map<TaskDTO>(task);
        }

        public async Task<TaskDTO> UpdateAsync(Guid id, UpdateTaskDTO dto)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
                throw new NotFoundException(nameof(TaskItem), id);

            // Business Rule: Completed tasks cannot be edited
            if (task.Status == TaskItemStatus.Completed)
                throw new ValidationException(
                    "Completed tasks cannot be edited");

            // Business Rule: DueDate >= StartDate
            if (dto.DueDate < dto.StartDate)
                throw new ValidationException(
                    "Due date cannot be earlier than start date");

            if (!Enum.TryParse<TaskPriority>(dto.Priority, out var priority))
                throw new ValidationException("Invalid priority value");

            if (!Enum.TryParse<TaskItemStatus>(dto.Status, out var status))
                throw new ValidationException("Invalid status value");

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = priority;
            task.Status = status;
            task.StartDate = dto.StartDate;
            task.DueDate = dto.DueDate;
            task.AssignedEmployeeId = dto.AssignedEmployeeId;
            task.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.CompleteAsync();

            // Notification — Task Completed
            if (status == TaskItemStatus.Completed)
            {
                var employee = await _unitOfWork.Employees
                    .GetByIdAsync(task.AssignedEmployeeId);
                if (employee != null)
                    await _notificationService.CreateNotificationAsync(
                        employee.UserId,
                        $"Task completed: {task.Title}");
            }

            return _mapper.Map<TaskDTO>(task);
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
                throw new NotFoundException(nameof(TaskItem), id);

            _unitOfWork.Tasks.Remove(task);
            await _unitOfWork.CompleteAsync();
        }
    }
}
