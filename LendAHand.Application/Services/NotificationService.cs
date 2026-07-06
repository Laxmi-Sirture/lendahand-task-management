using AutoMapper;
using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(
            Guid userId)
        {
            var notifications = await _unitOfWork.Notifications
                .GetUserNotificationsAsync(userId);
            return _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _unitOfWork.Notifications
                .GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _unitOfWork.Notifications.Update(notification);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task CreateNotificationAsync(Guid userId, string message)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.CompleteAsync();
        }
    }
}
