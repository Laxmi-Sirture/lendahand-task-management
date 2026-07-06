using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.DTOs
{
    public class TaskReportDTO
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string AssignedEmployeeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class EmployeeTaskReportDTO
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}
