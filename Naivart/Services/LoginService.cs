using Microsoft.IdentityModel.Tokens;
using Naivart.Models.APIModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class LoginService
    {
        private readonly string key;

        public LoginService()
        {
            this.key = "Santa123Doktor456BigShock789Komin987";
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
                var tokenKey = Encoding.ASCII.GetBytes(key);
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

        public bool LoginPasswordCheck(string name, string password)    //change this when Database is added
        {
            //return DbContext.Players.Any(x => x.Name == name && x.Password == password);
            return true;
        }
    }
}
