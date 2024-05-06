using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface ISchoolYearsRepository
    {
        public Task<IEnumerable<SchoolYearResponse>> GetAll();
        public Task<bool> AddSchoolYear(SchoolYearRequest schoolYear);
    }
}
