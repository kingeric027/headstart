using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Marketplace.Models.Models.Marketplace
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VerifiedUserType
    {
        supplier, admin, buyer, 
        
        // not a user type being returned from token, used to represent states without verified user
        noUser
    }
}
