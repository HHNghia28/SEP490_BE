using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class RegisterBookUpdateRequest
    {
        public string ID { get; set; }
        public string Note { get; set; }
        public string Rating { get; set; }
    }
}
