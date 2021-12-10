using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings appSettings;
        private ApplicationDbContext DbContext { get; }

        public AuthService(IOptions<AppSettings> appSettings, ApplicationDbContext dbcontext)
        {
            this.appSettings = appSettings.Value;
            DbContext = dbcontext;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var player = DbContext.Players.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            // return null if user not found
            if (player == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(player);

            return new AuthenticateResponse(player, token);
        }

        public IEnumerable<Player> GetAll()
        {
            return DbContext.Players.ToList();
        }

        public Player GetById(long id)
        {
            return DbContext.Players.FirstOrDefault(x => x.Id == id);
        }

        private string generateJwtToken(Player player)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new Claim[]
                //        {
                //            new Claim(ClaimTypes.Name, player.Username)
                //        }),
                Subject = new ClaimsIdentity(new[] { new Claim("id", player.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
