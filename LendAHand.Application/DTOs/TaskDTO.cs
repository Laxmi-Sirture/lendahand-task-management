using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.DTOs
{
    public class TaskDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public Guid AssignedEmployeeId { get; set; }
        public string AssignedEmployeeName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<FileUploadDTO> FileUploads { get; set; } = new();
    }

    public class CreateTaskDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public Guid AssignedEmployeeId { get; set; }
    }

    public class UpdateTaskDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public Guid AssignedEmployeeId { get; set; }
    }
}
