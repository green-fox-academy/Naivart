using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models;
using Naivart.Models.APIModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Web.Helpers;

namespace Naivart.Services
{
    public class LoginService
    {
        private readonly AppSettings appSettings;
        public AuthService AuthService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        public LoginService(IOptions<AppSettings> appSettings, AuthService authService, IUnitOfWork unitOfWork)
        {
            this.appSettings = appSettings.Value;
            AuthService = authService;
            UnitOfWork = unitOfWork;
        }
        public string Authenticate(PlayerLogin player, out int statusCode)
        {
            string username = player.Username;
            string password = player.Password;
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
            catch
            {
                statusCode = 500;
                return "Data could not be read";
            }
        }
        public bool IsLoginPasswordCorrect(string name, string password)    
        {
            try
            {
                var player = UnitOfWork.Players.FirstOrDefault(x => x.Username == name);
                if (player is null)
                {
                    return false;
                }
                password = password + player.Salt;  //connect input password with salt
                //check if input pasword is same as hashed password in database, return bool
                return Crypto.VerifyHashedPassword(player.Password, password);     
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
        public PlayerWithKingdom GetTokenOwner(PlayerIdentity player)
        {
            string token = player.Token;
            try
            {               
                var identity = AuthService.GetNameFromToken(token);
                return FindPlayerByName(identity);
            }
            catch
            {
                return null;
            }
        }
        public PlayerWithKingdom FindPlayerByName(string name)
        {
            try
            {
                var player = UnitOfWork.Players.Include(x => x.Kingdom).Where(x => x.Username == name).FirstOrDefault();


                PlayerWithKingdom playerWithKingdom = new PlayerWithKingdom 
                { KingdomId = player.KingdomId, KingdomName = player.Kingdom.Name, Ruler = player.Username };
                return playerWithKingdom;
            }
            catch
            {
                return null;
            }       
        }

        public PlayerInfo GetTokenOwnerInfo(string token)
        {
            try
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
                var identity = principal.Identity.Name;

                return FindPlayerByNameReturnPlayerInfo(identity);
            }
            catch
            {
                return null;
            }
        }

        public PlayerInfo FindPlayerByNameReturnPlayerInfo(string name)
        {
            var model = UnitOfWork.Players.FirstOrDefault(x => x.Username == name);
            return new PlayerInfo() { Id = model.Id, Username = model.Username, KingdomId = model.KingdomId};
        }
    }
}
