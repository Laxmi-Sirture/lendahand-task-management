using ClosedXML.Excel;
using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TaskReportDTO>> GetCompletedTasksReportAsync()
        {
            var tasks = await _unitOfWork.Tasks
                .GetByStatusesAsync(TaskItemStatus.Completed);   // ← FindAsync ki jagah ye

            return tasks.Select(t => new TaskReportDTO
            {
                TaskId = t.Id,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                AssignedEmployeeName = t.AssignedEmployee?.Name ?? "",
                StartDate = t.StartDate,
                DueDate = t.DueDate
            });
        }
        public async Task<IEnumerable<TaskReportDTO>> GetPendingTasksReportAsync()
        {
            var tasks = await _unitOfWork.Tasks
                .GetByStatusesAsync(TaskItemStatus.Pending, TaskItemStatus.InProgress);   // ← yahan bhi

            return tasks.Select(t => new TaskReportDTO
            {
                TaskId = t.Id,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                AssignedEmployeeName = t.AssignedEmployee?.Name ?? "",
                StartDate = t.StartDate,
                DueDate = t.DueDate
            });
        }
        public async Task<IEnumerable<EmployeeTaskReportDTO>> GetEmployeeWiseReportAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            var result = new List<EmployeeTaskReportDTO>();

            foreach (var emp in employees)
            {
                var tasks = await _unitOfWork.Tasks
                    .FindAsync(t => t.AssignedEmployeeId == emp.Id);

                result.Add(new EmployeeTaskReportDTO
                {
                    EmployeeName = emp.Name,
                    Department = emp.Department,
                    TotalTasks = tasks.Count(),
                    CompletedTasks = tasks.Count(
                        t => t.Status == TaskItemStatus.Completed),
                    PendingTasks = tasks.Count(
                        t => t.Status == TaskItemStatus.Pending),
                    OverdueTasks = tasks.Count(
                        t => t.Status != TaskItemStatus.Completed
                            && t.DueDate < DateTime.UtcNow)
                });
            }

            return result;
        }

        public async Task<byte[]> ExportToExcelAsync(string reportType)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");

            if (reportType == "completed")
            {
                var data = await GetCompletedTasksReportAsync();
                worksheet.Cell(1, 1).Value = "Title";
                worksheet.Cell(1, 2).Value = "Status";
                worksheet.Cell(1, 3).Value = "Priority";
                worksheet.Cell(1, 4).Value = "Employee";
                worksheet.Cell(1, 5).Value = "Start Date";
                worksheet.Cell(1, 6).Value = "Due Date";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.Title;
                    worksheet.Cell(row, 2).Value = item.Status;
                    worksheet.Cell(row, 3).Value = item.Priority;
                    worksheet.Cell(row, 4).Value = item.AssignedEmployeeName;
                    worksheet.Cell(row, 5).Value = item.StartDate.ToString("dd-MM-yyyy");
                    worksheet.Cell(row, 6).Value = item.DueDate.ToString("dd-MM-yyyy");
                    row++;
                }
            }
            else if (reportType == "employee")
            {
                var data = await GetEmployeeWiseReportAsync();
                worksheet.Cell(1, 1).Value = "Employee";
                worksheet.Cell(1, 2).Value = "Department";
                worksheet.Cell(1, 3).Value = "Total Tasks";
                worksheet.Cell(1, 4).Value = "Completed";
                worksheet.Cell(1, 5).Value = "Pending";
                worksheet.Cell(1, 6).Value = "Overdue";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.EmployeeName;
                    worksheet.Cell(row, 2).Value = item.Department;
                    worksheet.Cell(row, 3).Value = item.TotalTasks;
                    worksheet.Cell(row, 4).Value = item.CompletedTasks;
                    worksheet.Cell(row, 5).Value = item.PendingTasks;
                    worksheet.Cell(row, 6).Value = item.OverdueTasks;
                    row++;
                }
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportToCsvAsync(string reportType)
        {
            var sb = new StringBuilder();

            if (reportType == "completed")
            {
                var data = await GetCompletedTasksReportAsync();
                sb.AppendLine("Title,Status,Priority,Employee,StartDate,DueDate");
                foreach (var item in data)
                    sb.AppendLine($"{item.Title},{item.Status},{item.Priority}," +
                        $"{item.AssignedEmployeeName}," +
                        $"{item.StartDate:dd-MM-yyyy}," +
                        $"{item.DueDate:dd-MM-yyyy}");
            }
            else if (reportType == "employee")
            {
                var data = await GetEmployeeWiseReportAsync();
                sb.AppendLine("Employee,Department,Total,Completed,Pending,Overdue");
                foreach (var item in data)
                    sb.AppendLine($"{item.EmployeeName},{item.Department}," +
                        $"{item.TotalTasks},{item.CompletedTasks}," +
                        $"{item.PendingTasks},{item.OverdueTasks}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
