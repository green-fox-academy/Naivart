namespace Naivart.Models.APIModels
{
    public class ErrorResponse
    {
        public string Error { get; set; }

        public ErrorResponse(string error)
        {
            Error = error;
        }
    }
}
