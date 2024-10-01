//using ead_backend.Model;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace ead_backend.Healpers
//{
//    public class JwtHelper
//    {
//        private readonly IConfiguration _configuration;

//        public JwtHelper(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public string GenerateJwtToken(string email, string role)
//        {
//            var secretKey = _configuration["Jwt:SecretKey"];
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
//            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//        {
//            new Claim(JwtRegisteredClaimNames.Sub, email),
//            new Claim(ClaimTypes.Role, role),
//            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//        };

//            var token = new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"],
//                audience: _configuration["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationInHours"])),
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        public ClaimsPrincipal ValidateToken(string token)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
//            try
//            {
//                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(key),
//                    ValidateIssuer = true,
//                    ValidIssuer = _configuration["Jwt:Issuer"],
//                    ValidateAudience = true,
//                    ValidAudience = _configuration["Jwt:Audience"],
//                    ValidateLifetime = true,
//                    ClockSkew = TimeSpan.Zero
//                }, out SecurityToken validatedToken);

//                return principal;
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public string GetEmailFromToken(string token)
//        {
//            var principal = ValidateToken(token);
//            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//        }

//        public string GetRoleFromToken(string token)
//        {
//            var principal = ValidateToken(token);
//            return principal?.FindFirst(ClaimTypes.Role)?.Value;
//        }
//    }
//}

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ead_backend.Healpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Generate JWT token with userId, email, and role
        public string GenerateJwtToken(string email, string role, string userId)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", userId)  // Custom claim to include userId
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationInHours"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }
        public string GetEmailFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }

        public string GetRoleFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.Role)?.Value;
        }

        public string GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst("userId")?.Value;
        }
    }
}
