using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScheduleDetailResponse
    {
        public string ID { get; set; }
        public string Date { get; set; }
        public string WeekDate { get; set; }
        public List<ScheduleSlotResponse> Slots { get; set; }
    }
}
