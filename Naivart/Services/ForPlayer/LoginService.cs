using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Naivart.Database;
using Naivart.Models;
using Naivart.Models.APIModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Naivart.Services
{
    public class LoginService
    {
        private readonly AppSettings appSettings;
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public LoginService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext, AuthService authService)
        {
            this.appSettings = appSettings.Value;
            DbContext = dbContext;
            AuthService = authService;
        }
        public async Task<ValueTuple<int, string>> AuthenticateAsync(PlayerLogin player)
        {
            string username = player.Username;
            string password = player.Password;
            try
            {
                if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                {
                    //statusCode = 400;
                    return (400, "Field username and/or field password was empty!");
                }
                else if (await IsLoginPasswordCorrectAsync(username, password))
                {
                    //statusCode = 200;
                    return (200, AuthService.GetToken(username));
                }
                //statusCode = 401;
                return (401, "Username and/or password was incorrect!");
            }
            catch
            {
                //statusCode = 500;
                return (500, "Data could not be read");
            }
        }
        public async Task<bool> IsLoginPasswordCorrectAsync(string name, string password)    
        {
            try
            {
                var player = await Task.FromResult(DbContext.Players.FirstOrDefault(x => x.Username == name));
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
        public async Task<PlayerWithKingdom> GetTokenOwnerAsync(PlayerIdentity player)
        {
            string token = player.Token;
            try
            {               
                var identity = AuthService.GetNameFromToken(token);
                return await FindPlayerByNameAsync(identity);
            }
            catch
            {
                return null;
            }
        }
        public async Task<PlayerWithKingdom> FindPlayerByNameAsync(string name)
        {
            try
            {
                var player = await Task.FromResult(DbContext.Players.Where(x => x.Username == name)
                                             .Include(x => x.Kingdom).FirstOrDefault());
                PlayerWithKingdom playerWithKingdom = new PlayerWithKingdom 
                { KingdomId = player.KingdomId, KingdomName = player.Kingdom.Name, Ruler = player.Username };
                return playerWithKingdom;
            }
            catch
            {
                return null;
            }       
        }

        public async Task<PlayerInfo> GetTokenOwnerInfoAsync(string token)
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

                return await FindPlayerByNameReturnPlayerInfoAsync(identity);
            }
            catch
            {
                return null;
            }
        }

        public async Task<PlayerInfo> FindPlayerByNameReturnPlayerInfoAsync(string name)
        {
            var model = await Task.FromResult(DbContext.Players.FirstOrDefault(x => x.Username == name));
            return new PlayerInfo() { Id = model.Id, Username = model.Username, KingdomId = model.KingdomId};
        }
    }
}
