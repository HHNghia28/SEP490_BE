using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IRegisterBookRepository
    {
        public Task<RegistersBookResponse> GetRegistersBook(string classID, string fromDate);
        public Task UpdateRegisterBook(string accountID, RegisterBookUpdateRequest request);
        public Task<RegistersBookSlotResponse> GetRegisterBookForSlot(string scheduleID);
    }
}
