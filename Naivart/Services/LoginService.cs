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
        private readonly AppSettings appSettings;
        public LoginService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext)
        {
            this.appSettings = appSettings.Value;
            DbContext = dbContext;
        }

        public string Authenticate(PlayerLogin player)
        {
            string username = player.username;
            string password = player.password;
            try
            {
                if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                {
                    return "empty";
                }
                else if (IsLoginPasswordCorrect(username, password))
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
                    return "incorrect";
            }
            catch (Exception)
            {
                return "incorrect";
            }
        }
        public bool IsLoginPasswordCorrect(string name, string password)    
        {
            try
            {
                return DbContext.Players.Any(x => x.Username == name && x.Password == password);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
    }
}
