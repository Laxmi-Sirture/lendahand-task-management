using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.DTOs
{
    public class AdminDashboardDTO
    {
        public int TotalEmployees { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
    }

    public class EmployeeDashboardDTO
    {
        public int MyTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}
