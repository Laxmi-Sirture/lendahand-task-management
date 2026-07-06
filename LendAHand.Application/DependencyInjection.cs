using FluentValidation;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IFileService, FileService>();
            return services;
        }
    }
}
