using LendAHand.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByUserIdAsync(Guid userId);
        Task<(IEnumerable<Employee> Employees, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? search, string? sortBy, bool ascending);
    }
}
