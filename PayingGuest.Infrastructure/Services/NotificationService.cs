using Microsoft.Extensions.Logging;
using PayingGuest.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendNotificationAsync(int userId, string message, string type)
        {
            // Placeholder implementation - integrate with actual notification service
            _logger.LogInformation("Notification sent to user {UserId}: {Message}", userId, message);
            return Task.CompletedTask;
        }
    }
}
