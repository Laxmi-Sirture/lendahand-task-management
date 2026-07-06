using LendAHand.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<IEnumerable<TaskReportDTO>> GetCompletedTasksReportAsync();
        Task<IEnumerable<TaskReportDTO>> GetPendingTasksReportAsync();
        Task<IEnumerable<EmployeeTaskReportDTO>> GetEmployeeWiseReportAsync();
        Task<byte[]> ExportToExcelAsync(string reportType);
        Task<byte[]> ExportToCsvAsync(string reportType);
    }
}
