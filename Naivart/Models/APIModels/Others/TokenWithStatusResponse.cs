namespace Naivart.Models.APIModels
{
    public class TokenWithStatusResponse
    {
        public string Status { get; set; }
        public string Token { get; set; }

        public TokenWithStatusResponse(string status, string token)
        {
            Status = status;
            Token = token;
        }
    }
}
