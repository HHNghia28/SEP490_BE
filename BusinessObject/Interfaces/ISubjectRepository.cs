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
        public Task AddSubject(SubjectRequest request);
        public Task UpdateSubject(string subjectID, SubjectRequest request);
        public Task DeleteSubject(string subjectID);
    }
}
