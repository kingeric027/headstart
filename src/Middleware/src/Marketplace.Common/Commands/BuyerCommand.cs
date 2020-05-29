using Marketplace.Common.TemporaryAppConstants;
using Marketplace.Models;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceBuyerCommand
    {
        Task<MarketplaceBuyer> Create(MarketplaceBuyer buyer, VerifiedUserContext user, string token);
    }
    public class MarketplaceBuyerCommand : IMarketplaceBuyerCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;

        public MarketplaceBuyerCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }
        public async Task<MarketplaceBuyer> Create(MarketplaceBuyer buyer, VerifiedUserContext user, string token)
        {
            buyer.ID = "{buyerIncrementor}";
            buyer.Active = true;
            var ocBuyer = await _oc.Buyers.CreateAsync(buyer, token);
            buyer.ID = ocBuyer.ID;
            var ocBuyerID = ocBuyer.ID;

            // create base security profile assignment
            await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment
            {
                BuyerID = ocBuyerID,
                SecurityProfileID = CustomRole.MPBaseBuyer.ToString()
            }, token);

            await _oc.Incrementors.CreateAsync(new Incrementor { ID = $"{ocBuyerID}-UserIncrementor", LastNumber = 0, LeftPaddingCount = 5, Name = "User Incrementor"});
            await _oc.Incrementors.CreateAsync(new Incrementor { ID = $"{ocBuyerID}-LocationIncrementor", LastNumber = 0, LeftPaddingCount = 4, Name = "Location Incrementor" });

            return buyer;

        }
    }
}
