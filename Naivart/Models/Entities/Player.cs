using System.ComponentModel.DataAnnotations;

namespace Naivart.Models.Entities
{
    public class Player
    {
        [Key]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }

        public Player(string username, string password, string salt)
        {
            Username = username;
            Password = password;
            Salt = salt;
        }
    }
}
