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
        private readonly AppSettings appSettings;
        public LoginService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext)
        {
            this.appSettings = appSettings.Value;
            DbContext = dbContext;
        }

        public string Authenticate(PlayerLogin player, out int statusCode)
        {
            string username = player.username;
            string password = player.password;
            try
            {
                if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                {
                    statusCode = 400;
                    return "Field username and/or field password was empty!";
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
                    statusCode = 200;
                    return tokenHandler.WriteToken(token);  
                }
                statusCode = 401;
                return "Username and/or password was incorrect!";
            }
            catch (Exception)
            {
                statusCode = 500;
                return "Data could not be read";
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


        public PlayerWithKingdom GetTokenOwner(PlayerIdentity player)
        {
            string token = player.token;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Encoding.ASCII.GetBytes(appSettings.Key);

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
        public string CleanToken(string auth)
        {
            return auth.Remove(0,7);
        }
        public bool IsTokenOwner(string username, string auth)
        {
            string token = CleanToken(auth);
            var model = GetTokenOwner(new PlayerIdentity() { token = token});
            return model.ruler == username;
        }
        public bool IsTokenOwner(long id, string auth)
        {
            string token = CleanToken(auth);
            var model = GetTokenOwnerInfo(token);
            return model.Id == id;
        }

        public PlayerInfo GetTokenOwnerInfo(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Encoding.ASCII.GetBytes(appSettings.Key);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var identity = principal.Identity.Name;

                return FindPlayerByNameReturnPlayerInfo(identity);
            }

            catch (Exception)
            {
                return null;
            }
        }

        public PlayerInfo FindPlayerByNameReturnPlayerInfo(string name)
        {
            var model = DbContext.Players.FirstOrDefault(x => x.Username == name);
            return new PlayerInfo() { Id = model.Id, Username = model.Username, KingdomId = model.KingdomId};
        }


    }
}
