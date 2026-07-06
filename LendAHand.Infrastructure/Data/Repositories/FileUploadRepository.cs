using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Infrastructure.Data.Repositories
{
    public class FileUploadRepository : GenericRepository<FileUpload>, IFileUploadRepository
    {
        public FileUploadRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<FileUpload>> GetByTaskIdAsync(Guid taskId)
            => await _context.FileUploads
                .AsNoTracking()
                .Where(f => f.TaskId == taskId)
                .ToListAsync();
    }
}
