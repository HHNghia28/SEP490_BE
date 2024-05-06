using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Messages
    {
        private static Messages _instance;
        public static Messages Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Messages();
                }
                return _instance;
            }
        }

        public string SCHOOL_YEAR_DATE_1 = "Ngày kết thúc phải nằm trước ngày bắt đầu";
    }
}
