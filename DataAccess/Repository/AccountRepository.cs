using Azure.Core;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
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

        public AccountRepository(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private string CreateToken(LoginResponse user, int seconds)
        {
            string issuer = _configuration["AppSettings:Issuer"];
            string audience = _configuration["AppSettings:Audience"];
            string secretKey = _configuration["AppSettings:SecretKey"];

            List<Claim> authClaims = new List<Claim>
            {
                new Claim("ID", user.User.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

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
