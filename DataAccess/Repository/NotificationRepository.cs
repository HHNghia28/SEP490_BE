using Azure.Core;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using BusinessObject.IServices;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IImageService _imageService;

        public NotificationRepository(ApplicationDbContext context, IActivityLogRepository activityLogRepository, IImageService imageService)
        {
            _context = context;
            _activityLogRepository = activityLogRepository;
            _imageService = imageService;
        }

        public async Task AddNotification(string accountID, NotificationRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Notification notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Title.ToLower().Equals(request.Title.ToLower().Trim()));

            if (notification != null)
            {
                throw new ArgumentException("Thông báo " + request.Title + " đã tồn tại");
            }

            string avt = "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png";

            if (request.Thumbnail != null)
            {
                avt = await _imageService.UploadImage(request.Thumbnail, "Notification");
            }

            await _context.Notifications.AddAsync(new Notification()
            {
                ID = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Thumbnail = avt,
                Content = request.Content.Trim(),
                CreateBy = account.ID,
            });

            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện thêm thông báo " + request.Title.Trim(),
                Type = LogName.CREATE.ToString(),
            });
        }

        public async Task DeleteNotification(string accountID, string notiID)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Notification notification = await _context.Notifications
                .FirstOrDefaultAsync(n => Guid.Equals(n.ID, new Guid(notiID))) ?? throw new NotFoundException("Không tìm thấy thông báo");

            _context.Notifications.Remove(notification);

            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện xóa thông báo " + notification.Title.Trim(),
                Type = LogName.DELETE.ToString(),
            });
        }

        public async Task<IEnumerable<NotificationResponse>> GetAllNotifications()
        {
            return await _context.Notifications
                .Include(n => n.Account)
                .OrderByDescending(n => n.UpdateAt)
                .Select(n => new NotificationResponse()
                {
                    ID = n.ID,
                    Title = n.Title,
                    Content = n.Content,
                    Thumbnail = n.Thumbnail,
                    UpdateAt = n.UpdateAt.ToString("dd/MM/yyyy"),
                    CreateAt = n.CreateAt.ToString("dd/MM/yyyy"),
                    CreateBy = n.Account.Username,
                })
                .ToListAsync();
        }

        public async Task<NotificationResponse> GetNotification(string notiID)
        {
            Notification n = await _context.Notifications
                .Include(n => n.Account)
                .FirstOrDefaultAsync(n => Guid.Equals(n.ID, new Guid(notiID))) ?? throw new NotFoundException("Không tìm thấy thông báo");

            return new NotificationResponse()
            {
                ID = n.ID,
                Title = n.Title,
                Content = n.Content,
                Thumbnail = n.Thumbnail,
                UpdateAt = n.UpdateAt.ToString("dd/MM/yyyy"),
                CreateAt = n.CreateAt.ToString("dd/MM/yyyy"),
                CreateBy = n.Account.Username,
            };
        }

        public async Task UpdateNotification(string accountID, string notiID, NotificationRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Notification notification = await _context.Notifications
                .FirstOrDefaultAsync(n => Guid.Equals(n.ID, new Guid(notiID))) ?? throw new NotFoundException("Không tìm thấy thông báo");

            string avt = notification.Thumbnail;

            if (request.Thumbnail != null)
            {
                avt = await _imageService.UploadImage(request.Thumbnail, "Notification");
            }

            notification.Thumbnail = avt;
            notification.Title = request.Title.Trim();
            notification.Content = request.Content.Trim();
            notification.CreateBy = account.ID;
            notification.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện cập nhật thông báo " + request.Title.Trim(),
                Type = LogName.UPDATE.ToString(),
            });
        }
    }
}
