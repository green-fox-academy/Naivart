using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Interfaces.ServiceInterfaces;
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
    public class LoginService : ILoginService
    {
        private readonly AppSettings _appSettings;
        public IAuthService AuthService { get; set; }
        private IUnitOfWork _unitOfWork { get; set; }
        public LoginService(IOptions<AppSettings> appSettings, IAuthService authService, IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings.Value;
            AuthService = authService;
            _unitOfWork = unitOfWork;
        }
        public async Task<(int status, string message)> AuthenticateAsync(PlayerLogin player)
        {
            string username = player.Username;
            string password = player.Password;
            try
            {
                if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                {
                    return (400, "Field username and/or field password was empty!");
                }
                else if (await IsLoginPasswordCorrectAsync(username, password))
                {
                    return (200, AuthService.GetToken(username));
                }
                return (401, "Username and/or password was incorrect!");
            }
            catch
            {
                return (500, "Data could not be read");
            }
        }
        public async Task<bool> IsLoginPasswordCorrectAsync(string name, string password)    
        {
            try
            {
                if (await _unitOfWork.Players.FindByUsernameAsync(name) is null)
                {
                    return false;
                }
                password += (await _unitOfWork.Players.FindByUsernameAsync(name)).Salt;  //connect input password with salt
                //check if input pasword is same as hashed password in database, return bool
                return Crypto.VerifyHashedPassword((await _unitOfWork.Players.FindByUsernameAsync(name)).Password, password);     
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
                return await FindPlayerByNameAsync(AuthService.GetNameFromToken(token));
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
                var player = await _unitOfWork.Players.FindPlayerIncludeKingdomsByUsernameAsync(name);
                return new PlayerWithKingdom(player.KingdomId, player.Kingdom.Name, player.Username);
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
                var symmetricKey = Encoding.ASCII.GetBytes(_appSettings.Key);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var identity = principal.Identity.Name;

                return await _unitOfWork.Players.FindPlayerByNameReturnPlayerInfoAsync(identity);
            }
            catch
            {
                return null;
            }
        }
    }
}
