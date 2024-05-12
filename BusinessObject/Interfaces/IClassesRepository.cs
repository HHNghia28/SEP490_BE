using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IClassesRepository
    {
        public Task<IEnumerable<ClassesResponse>> GetClasses();
        public Task<ClassResponse> GetClass(string classID);
        public Task AddClasses(ClassesRequest request);
        public Task UpdateClasses(string classID, ClassesRequest request);
        public Task DeleteClasses(string classID);
    }
}
