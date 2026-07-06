using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Domain.Entities
{
    public class FileUpload
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public TaskItem Task { get; set; } = null!;
    }
}
