using LendAHand.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<AdminDashboardDTO> GetAdminDashboardAsync();
        Task<EmployeeDashboardDTO> GetEmployeeDashboardAsync(Guid employeeId);
    }
}
