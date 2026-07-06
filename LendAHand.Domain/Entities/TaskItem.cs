using LendAHand.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskItemStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public Guid AssignedEmployeeId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueSoonNotifiedAt { get; set; }
        public Employee AssignedEmployee { get; set; } = null!;
        public ICollection<FileUpload> FileUploads { get; set; } = new List<FileUpload>();
    }
}
