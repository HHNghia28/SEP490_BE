using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class RegistersBookSlotResponse
    {
        public string ID { get; set; }
        public string Subject { get; set; }
        public string Teacher { get; set; }
        public int SlotByLessonPlan { get; set; }
        public int Slot {  get; set; }
        public int NumberOfAbsent {  get; set; }
        public List<string> NumberAbsent { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public string Rating { get; set; }
    }
}
