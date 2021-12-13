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
        public AuthService AuthService { get; set; }
        public LoginService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext, AuthService authService)
        {
            this.appSettings = appSettings.Value;
            DbContext = dbContext;
            AuthService = authService;
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
                    statusCode = 200;
                    return AuthService.GetToken(username);
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
                var identity = AuthService.GetNameFromToken(token);
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
        public bool IsTokenOwner(long id, string auth)
        {
            string token = AuthService.CleanToken(auth);
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
