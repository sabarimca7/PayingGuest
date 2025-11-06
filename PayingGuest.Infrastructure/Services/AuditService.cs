using Microsoft.AspNetCore.Http;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string tableName, int recordId, string action, object? oldValues, object? newValues)
        {
            var auditLog = new AuditLog
            {
                TableName = tableName,
                RecordId = recordId,
                Action = action,
                OldValues = oldValues != null ? System.Text.Json.JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? System.Text.Json.JsonSerializer.Serialize(newValues) : null,
                UserId = _currentUserService.UserId != null ? int.Parse(_currentUserService.UserId) : null,
                UserName = _currentUserService.UserName,
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                UserAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.AuditLogs.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
