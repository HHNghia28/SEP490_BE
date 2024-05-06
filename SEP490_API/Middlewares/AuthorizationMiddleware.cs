using BusinessObject.Entities;
using BusinessObject.Interfaces;
using DataAccess.Context;
using SEP490_API.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SEP490_API.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, IAuthorizationService authorizationService, IServiceScopeFactory serviceScopeFactory)
        {
            var tokenHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var token = tokenHeader?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                ClaimsPrincipal principal;
                try
                {
                    principal = ValidateToken(token);
                }
                catch (SecurityTokenExpiredException)
                {
                    APIResponse<string> aPIResponse = new()
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized",
                        Success = false,
                        Data = "Token đã quá hạn."
                    };

                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(aPIResponse);

                    context.Response.ContentType = "application/json";

                    context.Response.StatusCode = StatusCodes.Status200OK;

                    await context.Response.WriteAsync(jsonResponse);
                    return;
                }

                var userIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == "ID" || c.Type == ClaimTypes.NameIdentifier);
            }

            await _next(context);
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            string secretKey = _configuration["AppSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false, 
                ValidIssuer = _configuration["AppSettings:Issuer"],
                ValidAudience = _configuration["AppSettings:Audience"],
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

            return principal;
        }
    }
}
