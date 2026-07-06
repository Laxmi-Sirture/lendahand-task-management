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
    public class NotificationRepository :
       GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(
            Guid userId)
            => await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            notifications.ForEach(n => n.IsRead = true);
        }
    }
}
