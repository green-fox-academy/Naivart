using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models
{
    public class AuthenticateResponse
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public long KingdomId { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(Player player, string token)
        {
            Id = player.Id;
            Username = player.Username;
            KingdomId = player.KingdomId;
            Token = token;
        }
    }
}
