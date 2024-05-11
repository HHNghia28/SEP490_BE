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
        Task UpdateTeacher(RegisterTeacherRequest request);
    }
}
