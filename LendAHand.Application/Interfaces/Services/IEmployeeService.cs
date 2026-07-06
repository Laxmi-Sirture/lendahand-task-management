using LendAHand.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<PagedResultDTO<EmployeeDTO>> GetAllAsync(
            int page, int pageSize,
            string? search, string? sortBy, bool ascending);
        Task<EmployeeDTO?> GetByIdAsync(Guid id);
        Task<EmployeeDTO> CreateAsync(CreateEmployeeDTO dto);
        Task<EmployeeDTO> UpdateAsync(Guid id, UpdateEmployeeDTO dto);

        Task<EmployeeDTO?> GetByUserIdAsync(Guid userId);
        Task DeleteAsync(Guid id);
    }
}
