using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Naivart.Database;
using Naivart.Models;
using Naivart.Models.APIModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Naivart.Services
{
    public class LoginService
    {
        private ApplicationDbContext DbContext { get; }
        private readonly AppSettings _appSettings;
        public LoginService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        public string Authenticate(PlayerLogin player)
        {
            string username = player.username;
            string password = player.password;

            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                return "";
            }
            else if (LoginPasswordCheck(username, password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes(_appSettings.key);
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
            else
            {
                return null;
            }
        }

        public bool LoginPasswordCheck(string name, string password)    
        {
            return DbContext.Players.Any(x => x.Username == name && x.Password == password);
        }
        public PlayerWithKingdom GetPrincipal(PlayerIdentity player)
        {
            string token = player.token;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Encoding.ASCII.GetBytes(_appSettings.key);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var identity = principal.Identity.Name;

                return FindPlayerByName(identity);
            }

            catch (Exception)
            {
                return null;
            }
        }
        public PlayerWithKingdom FindPlayerByName(string name)
        {
            try
            {
                var player = DbContext.Players.Where(x => x.Username == name).Include(x => x.Kingdom).FirstOrDefault();
                PlayerWithKingdom playerWithKingdom = new PlayerWithKingdom { kingdomId = player.KingdomId, kingdomName = player.Kingdom.Name, ruler = player.Username };
                return playerWithKingdom;
            }
            catch
            {
                return null;
            }       
        }
    }
}
