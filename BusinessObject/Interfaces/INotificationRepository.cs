using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface INotificationRepository
    {
        public Task AddNotification(string accountID, NotificationRequest request);
        public Task UpdateNotification(string accountID, string notiID, NotificationRequest request);
        public Task DeleteNotification(string accountID, string notiID);
        public Task<NotificationResponse> GetNotification(string notiID);
        public Task<IEnumerable<NotificationResponse>> GetAllNotifications();

    }
}
