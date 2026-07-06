using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IEmployeeRepository Employees { get; }
        ITaskRepository Tasks { get; }
        INotificationRepository Notifications { get; }
        IFileUploadRepository FileUploads { get; }   // ← ye line add karo

        Task<int> CompleteAsync();
    }
}
