using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface ISubjectRepository
    {
        public Task<IEnumerable<SubjectsResponse>> GetSubjects();
        public Task<SubjectResponse> GetSubject(string subjectID);
        public Task AddSubject(string accountID, SubjectRequest request);
        public Task AddSubjectsByExcel(string accountID, ExcelRequest request);
        public Task UpdateSubject(string accountID, string subjectID, SubjectRequest request);
        public Task DeleteSubject(string accountID, string subjectID);
    }
}
