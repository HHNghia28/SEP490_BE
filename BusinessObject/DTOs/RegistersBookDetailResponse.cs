using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class RegistersBookDetailResponse
    {
        public string ID { get; set; }
        public string Date { get; set; }
        public string WeekDate { get; set; }
        public List<RegistersBookSlotResponse> Slots { get; set; }
    }
}
