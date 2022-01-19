namespace Naivart.Models.APIModels
{
    public class RegisterResponse
    {
        public string Username { get; set; }
        public long KingdomId { get; set; }

        public RegisterResponse(string username, long kingdomId)
        {
            Username = username;
            KingdomId = kingdomId;
        }
    }
}
