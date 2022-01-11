using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Naivart.Database;
using Naivart.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class AuthService
    {
        private readonly AppSettings appSettings;
        private ApplicationDbContext DbContext { get; }
        public AuthService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext)
        {
            this.appSettings = appSettings.Value;
            DbContext = dbContext;
        }

        public string GetToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(appSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                            new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GetNameFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return null;
            }
            var symmetricKey = Encoding.ASCII.GetBytes(appSettings.Key);

            var validationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal.Identity.Name;
        }

        public async Task<bool> IsKingdomOwnerAsync(long kingdomId, string username)
        {
            try
            {
                return await Task.FromResult(DbContext.Players.FirstOrDefault(x => x.KingdomId == kingdomId).Username == username);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
    }
}
