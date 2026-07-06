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
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Employee?> GetByUserIdAsync(Guid userId)
            => await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

        public async Task<(IEnumerable<Employee> Employees, int TotalCount)>
            GetPagedAsync(int page, int pageSize,
            string? search, string? sortBy, bool ascending)
        {
            var query = _context.Employees.AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
                query = query.Where(e =>
                    e.Name.Contains(search) ||
                    e.Email.Contains(search) ||
                    e.Department.Contains(search) ||
                    e.Designation.Contains(search));

            // Sort
            query = sortBy switch
            {
                "name" => ascending
                    ? query.OrderBy(e => e.Name)
                    : query.OrderByDescending(e => e.Name),
                "department" => ascending
                    ? query.OrderBy(e => e.Department)
                    : query.OrderByDescending(e => e.Department),
                "email" => ascending
                    ? query.OrderBy(e => e.Email)
                    : query.OrderByDescending(e => e.Email),
                _ => query.OrderByDescending(e => e.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var employees = await query
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, totalCount);
        }
    }
}
