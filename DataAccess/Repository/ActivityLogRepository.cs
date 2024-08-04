using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivityLogResponse>> GetLog()
        {
            return await _context.ActivityLogs
                .OrderByDescending(item => item.Date)
                .Select(item => new ActivityLogResponse()
                {
                    ID = item.ID,
                    Date = item.Date.ToString("HH:mm dd/MM/yyyy"), 
                    Note = item.Note,
                    Type = item.Type
                })
                .ToListAsync();
        }

        public async Task WriteLogAsync(ActivityLogRequest request)
        {
            ActivityLog activityLog = new()
            {
                ID = Guid.NewGuid(),
                AccountID = request.AccountID,
                Date = DateTime.Now,
                Note = request.Note,
                Title = "",
                Type = request.Type,
            };

            await _context.ActivityLogs.AddAsync(activityLog);
            await _context.SaveChangesAsync();
        }
    }
}
