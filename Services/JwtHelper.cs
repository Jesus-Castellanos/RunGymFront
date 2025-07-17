using Microsoft.IdentityModel.Tokens;
using RunGymFront.Models;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RunGymFront.Services
{
    public class JwtHelper
    {
        string secretKey = ConfigurationManager.AppSettings["JwtKey"].ToString();
        string validAudience = ConfigurationManager.AppSettings["JwtAudience"].ToString();
        string validIssuer = ConfigurationManager.AppSettings["JwtIssuer"].ToString();

        public ClaimsPrincipal ValidateToken(Token token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = true,
                ValidIssuer = validIssuer,

                ValidateAudience = true,
                ValidAudience = validAudience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token.token, validationParameters, out validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token inválido: {ex.Message}");
                return null;
            }
        }
    }
}