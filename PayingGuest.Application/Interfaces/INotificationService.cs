using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, string message, string type);
    }
}
