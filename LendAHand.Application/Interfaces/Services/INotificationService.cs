using LendAHand.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task CreateNotificationAsync(Guid userId, string message);
    }
}
