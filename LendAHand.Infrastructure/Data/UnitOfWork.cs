using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; }
        public IEmployeeRepository Employees { get; }
        public ITaskRepository Tasks { get; }
        public INotificationRepository Notifications { get; }

        public IFileUploadRepository FileUploads { get; }   // ← add


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(context);
            Employees = new EmployeeRepository(context);
            Tasks = new TaskRepository(context);
            Notifications = new NotificationRepository(context);
            FileUploads = new FileUploadRepository(context);
        }

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
