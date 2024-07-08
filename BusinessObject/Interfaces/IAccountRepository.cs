using BusinessObject.DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IAccountRepository
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<LoginResponse> RefreshToken(string accessToken, string refreshToken);
        Task RegisterTeacher(RegisterTeacherRequest request);
        Task UpdateTeacher(string accountID, UpdateTeacherRequest request);
        Task<IEnumerable<RegisterResponse>> GetTeachers();
        Task<TeacherResponse> GetTeacher(string accountID);
        Task DeleteTeacher(string accountID);
        Task RegisterStudent(RegisterStudentRequest request);
        Task<IEnumerable<RegisterResponse>> GetStudents();
        Task<StudentResponse> GetStudent(string accountID);
        Task DeleteStudent(string accountID);
        Task UpdateStudent(string accountID, UpdateStudentRequest request);
        Task AddStudentByExcel(string accountID, ExcelRequest request);
        Task AddTeacherByExcel(string accountID, ExcelRequest request);
    }
}
