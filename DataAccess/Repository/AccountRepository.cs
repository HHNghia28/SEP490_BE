﻿using Azure.Core;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using BusinessObject.IServices;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccess.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public AccountRepository(IConfiguration configuration, ApplicationDbContext context, IImageService imageService)
        {
            _configuration = configuration;
            _context = context;
            _imageService = imageService;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            Account accountExist = await _context.Accounts
                .Include(a => a.User)
                .Include(a => a.AccountRoles)
                .ThenInclude(a => a.Role)
                .ThenInclude(a => a.RolePermissions)
                .Include(a => a.AccountPermissions)
                .ThenInclude(a => a.Permission)
                .FirstOrDefaultAsync(a => a.Username.ToLower()
                .Equals(request.Username.ToLower()));

            if (accountExist == null)
            {
                AccountStudent accountStudentExist = await _context.AccountStudents
                .Include(a => a.Student)
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Username.ToLower()
                .Equals(request.Username.ToLower()))
                ?? throw new ArgumentException("Tên đăng nhập hoặc tài khoản không chính xác");

                if (!accountStudentExist.IsActive)
                {
                    throw new ArgumentException("Tài khoản chờ xác nhận");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, accountStudentExist.Password))
                {
                    throw new ArgumentException("Tên đăng nhập hoặc tài khoản không chính xác");
                }

                List<string> roleStudents = new();

                foreach (var item1 in accountStudentExist.Role.RolePermissions)
                {
                    roleStudents.Add(item1.Permission.Name);
                }

                string refreshTokenS = GenerateRefreshToken();

                accountStudentExist.RefreshToken = refreshTokenS;
                accountStudentExist.RefreshTokenExpires = DateTime.Now.AddDays(30);

                await _context.SaveChangesAsync();

                LoginResponse loginResponseS = new LoginResponse()
                {
                    User = new RegisterResponse()
                    {
                        Id = accountStudentExist.ID,
                        Username = accountStudentExist.Username,
                        Address = accountStudentExist.Student.Address,
                        Email = accountStudentExist.Student.Email,
                        Fullname = accountStudentExist.Student.Fullname,
                        Phone = accountStudentExist.Student.Phone,
                        Avatar = accountStudentExist.Student.Avatar
                    }
                };

                loginResponseS.AccessToken = CreateToken(loginResponseS, 60 * 60 * 24, roleStudents);
                loginResponseS.RefreshToken = refreshTokenS;

                return loginResponseS;
            }

            if (!accountExist.IsActive)
            {
                throw new ArgumentException("Tài khoản chờ xác nhận");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, accountExist.Password))
            {
                throw new ArgumentException("Tên đăng nhập hoặc tài khoản không chính xác");
            }

            List<string> roles = new();

            foreach (var item in accountExist.AccountRoles)
            {
                foreach (var item1 in item.Role.RolePermissions)
                {
                    roles.Add(item1.Permission.Name);
                }
            }

            foreach (var item in accountExist.AccountPermissions)
            {
                roles.Add(item.Permission.Name);
            }

            string refreshToken = GenerateRefreshToken();

            accountExist.RefreshToken = refreshToken;
            accountExist.RefreshTokenExpires = DateTime.Now.AddDays(30);

            await _context.SaveChangesAsync();

            LoginResponse loginResponse = new LoginResponse()
            {
                User = new RegisterResponse()
                {
                    Id = accountExist.ID,
                    Username = accountExist.Username,
                    Address = accountExist.User.Address,
                    Email = accountExist.User.Email,
                    Fullname = accountExist.User.Fullname,
                    Phone = accountExist.User.Phone,
                    Avatar = accountExist.User.Avatar
                }
            };

            loginResponse.AccessToken = CreateToken(loginResponse, 60 * 60 * 24, roles);
            loginResponse.RefreshToken = refreshToken;

            return loginResponse;
        }

        public async Task<LoginResponse> RefreshToken(string accessToken, string refreshToken)
        {
            string accountID = GetIdFromExpiredToken(accessToken);

            Account accountExist = await _context.Accounts
                .Include(a => a.User)
                .Include(a => a.User)
                .Include(a => a.AccountRoles)
                .ThenInclude(a => a.Role)
                .ThenInclude(a => a.RolePermissions)
                .Include(a => a.AccountPermissions)
                .ThenInclude(a => a.Permission)
                .FirstOrDefaultAsync(a => a.ID.Equals(accountID) && a.IsActive)
                ?? throw new ArgumentException("AccessToken không chính xác");

            if (accountExist.RefreshToken == null)
            {
                throw new ArgumentException("RefreshToken quá hạn");
            }

            List<string> roles = new();

            foreach (var item in accountExist.AccountRoles)
            {
                foreach (var item1 in item.Role.RolePermissions)
                {
                    roles.Add(item1.Permission.Name);
                }
            }

            foreach (var item in accountExist.AccountPermissions)
            {
                roles.Add(item.Permission.Name);
            }

            string newRefreshToken = GenerateRefreshToken();

            TimeSpan? timeSpan = accountExist.RefreshTokenExpires - DateTime.Now;
            if (accountExist.RefreshToken.Equals(refreshToken) && timeSpan.HasValue && timeSpan.Value.TotalDays < 30)
            {
                accountExist.RefreshToken = newRefreshToken;
                accountExist.RefreshTokenExpires = DateTime.Now.AddDays(30);

                await _context.SaveChangesAsync();

                LoginResponse loginResponse = new LoginResponse()
                {
                    User = new RegisterResponse()
                    {
                        Id = accountExist.ID,
                        Username = accountExist.Username,
                        Address = accountExist.User.Address,
                        Email = accountExist.User.Email,
                        Fullname = accountExist.User.Fullname,
                        Phone = accountExist.User.Phone,
                        Avatar = accountExist.User.Avatar
                    }
                };

                loginResponse.AccessToken = CreateToken(loginResponse, 60 * 60 * 8, roles);
                loginResponse.RefreshToken = newRefreshToken;

                return loginResponse;
            }

            throw new ArgumentException("RefreshToken quá hạn");
        }

        public async Task RegisterTeacher(RegisterTeacherRequest request)
        {
            Account accountIDExist = await _context.Accounts.FirstOrDefaultAsync(a => a.ID.ToLower()
            .Equals(request.ID.ToLower().Trim()));

            if (accountIDExist != null)
            {
                throw new ArgumentException("ID đã được sử dụng");
            }

            Account accountExist = await _context.Accounts.FirstOrDefaultAsync(a => a.Username.ToLower()
            .Equals(request.Username.ToLower().Trim()));

            if (accountExist != null)
            {
                throw new ArgumentException("Tên đăng nhập đã được sử dụng");
            }

            AccountStudent studentExist = await _context.AccountStudents.FirstOrDefaultAsync(a => a.Username.ToLower()
            .Equals(request.Username.ToLower().Trim()));

            if (studentExist != null)
            {
                throw new ArgumentException("Tên đăng nhập đã được sử dụng");
            }

            Guid guid = Guid.NewGuid();
            string accountID = request.ID;

            string avt = "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png";

            if (request.Avatar != null)
            {
                avt = await _imageService.UploadImage(request.Avatar);
            }

            User user = new()
            {
                ID = guid,
                Address = request.Address.Trim(),
                Avatar = avt,
                Email = request.Email.Trim(),
                Birthday = request.Birthday,
                Fullname = request.Fullname.Trim(),
                Gender = request.Gender.Trim(),
                IsBachelor = request.IsBachelor,
                IsDoctor = request.IsDoctor,
                IsMaster = request.IsMaster,
                IsProfessor = request.IsProfessor,
                Nation = request.Nation.Trim(),
                Phone = request.Phone.Trim(),
            };

            await _context.Users.AddAsync(user);

            Account account = new()
            {
                ID = accountID,
                IsActive = true,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password.Trim()),
                Username = request.Username.Trim(),
                UserID = guid,
                RefreshToken = "",
                RefreshTokenExpires = DateTime.Now
            };

            await _context.Accounts.AddAsync(account);

            List<AccountRole> roles = new();

            foreach (var role in request.Roles)
            {
                Role role1 = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower().Equals(role.ToLower()));

                if(role1 != null)
                {
                    roles.Add(new()
                    {
                        AccountID = accountID,
                        RoleID = role1.ID
                    });
                }
            }

            await _context.AccountRoles.AddRangeAsync(roles);

            List<AccountPermission> permissions = new();

            foreach (var role in request.Permissions)
            {
                Permission per = await _context.Permissions.FirstOrDefaultAsync(r => r.Name.ToLower().Equals(role.ToLower()));

                if(per != null)
                {
                    permissions.Add(new()
                    {
                        AccountID = accountID,
                        PermissionID = per.ID
                    });
                }
            }

            await _context.AccountPermissions.AddRangeAsync(permissions);
            await _context.SaveChangesAsync();
        }

        public string CreateNewAccountId()
        {
            var maxId = _context.Accounts.Max(a => a.ID);

            if (!string.IsNullOrEmpty(maxId))
            {
                var numericId = maxId.Substring(2);

                var number = int.Parse(numericId);

                number++;

                var newId = $"GV{number.ToString().PadLeft(4, '0')}";

                return newId;
            }

            return "GV0001";
        }

        private string CreateToken(LoginResponse user, int seconds, List<string> roles)
        {
            string issuer = _configuration["AppSettings:Issuer"];
            string audience = _configuration["AppSettings:Audience"];
            string secretKey = _configuration["AppSettings:SecretKey"];

            List<Claim> authClaims = new List<Claim>
            {
                new("ID", user.User.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: authClaims,
                expires: DateTime.UtcNow.AddSeconds(seconds),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Base64UrlEncode(randomNumber);
            }
        }

        private string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; 
            output = output.Replace('+', '-');
            output = output.Replace('/', '_'); 
            return output;
        }

        private string GetIdFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["AppSettings:Issuer"],
                ValidAudience = _configuration["AppSettings:Audience"],
                ValidateLifetime = false, 
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            var idClaim = principal.FindFirst("ID")?.Value;

            if (idClaim == null)
            {
                throw new SecurityTokenException("Invalid token claims");
            }

            return idClaim;
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            string secretKey = _configuration["AppSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["AppSettings:Issuer"],
                ValidAudience = _configuration["AppSettings:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

            return principal;
        }
    }
}
