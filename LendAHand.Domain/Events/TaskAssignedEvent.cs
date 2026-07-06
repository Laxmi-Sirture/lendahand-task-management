using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Domain.Events
{
    public class TaskAssignedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public Guid AssignedEmployeeId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
