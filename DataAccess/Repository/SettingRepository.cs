using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDbContext _context;

        public SettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SettingRequest> GetSetting()
        {
            SchoolSetting setting = await _context.SchoolSettings.FirstOrDefaultAsync();

            if (setting == null) return new SettingRequest();

            return new SettingRequest()
            {
                SchoolAddress = setting.SchoolAddress,
                SchoolEmail = setting.SchoolEmail,
                SchoolLevel = setting.SchoolLevel,
                SchoolName = setting.SchoolName,
                SchoolPhone = setting.SchoolPhone
            };
        }

        public async Task UpdateSetting(string accountID, SettingRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            List<SchoolSetting> settings = await _context.SchoolSettings.ToListAsync();

            _context.SchoolSettings.RemoveRange(settings);

            await _context.SchoolSettings.AddAsync(new SchoolSetting()
            {
                CreateBy = account.ID,
                SchoolAddress = request.SchoolAddress,
                SchoolEmail = request.SchoolEmail,
                SchoolLevel = request.SchoolLevel,
                SchoolName = request.SchoolName,
                SchoolPhone = request.SchoolPhone
            });

            await _context.SaveChangesAsync();
        }
    }
}
