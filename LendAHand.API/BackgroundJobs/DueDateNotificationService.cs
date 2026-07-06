using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Application.Interfaces.Services;

namespace LendAHand.API.BackgroundJobs
{
    public class DueDateNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DueDateNotificationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public DueDateNotificationService(
            IServiceScopeFactory scopeFactory,
            ILogger<DueDateNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckDueTomorrowTasksAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Due-date notification job failed");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckDueTomorrowTasksAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var dueTasks = await unitOfWork.Tasks.GetTasksDueTomorrowAsync();

            foreach (var task in dueTasks)
            {
                if (task.DueSoonNotifiedAt != null) continue;
                if (task.AssignedEmployee == null) continue;

                await notificationService.CreateNotificationAsync(
                    task.AssignedEmployee.UserId,
                    $"Task due tomorrow: {task.Title}");

                task.DueSoonNotifiedAt = DateTime.UtcNow;
                unitOfWork.Tasks.Update(task);
            }

            await unitOfWork.CompleteAsync();
            _logger.LogInformation("Due-date notification job ran, checked {Count} tasks", dueTasks.Count());
        }
    }
}