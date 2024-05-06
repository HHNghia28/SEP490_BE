using BusinessObject.DTOs;
using BusinessObject.Interfaces;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace DataAccess.Repository
{
    public class SchoolYearsRepository : ISchoolYearsRepository
    {
        private readonly ApplicationDbContext _context;

        public SchoolYearsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddSchoolYear(SchoolYearRequest schoolYear)
        {
            if (schoolYear.ToDate > schoolYear.FromDate)
            {
                throw new ArgumentException(Messages.Instance.SCHOOL_YEAR_DATE_1);
            }

            return false;
        }

        public async Task<IEnumerable<SchoolYearResponse>> GetAll()
        {
            return await _context.SchoolYears.Select(x => new SchoolYearResponse
            {
                Name = x.Name,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
            }).ToListAsync();
        }
    }
}
