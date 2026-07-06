using LendAHand.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Repositories
{
    public interface IFileUploadRepository : IGenericRepository<FileUpload>
    {
        Task<IEnumerable<FileUpload>> GetByTaskIdAsync(Guid taskId);
    }
}
