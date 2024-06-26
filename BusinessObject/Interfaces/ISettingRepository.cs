using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface ISettingRepository
    {
        Task<SettingRequest> GetSetting();
        Task UpdateSetting(string accountID, SettingRequest request);
    }
}
