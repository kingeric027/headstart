
namespace Marketplace.Common.Services.CardConnect.Models
{
    public class AccountRequest
    {
        public string account { get; set; }
    }


    public class AccountResponse
    {
        public string message { get; set; }
        public int errorcode { get; set; }
        public string token { get; set; }
    }

}
