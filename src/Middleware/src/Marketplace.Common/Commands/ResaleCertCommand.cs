using OrderCloud.SDK;
using ordercloud.integrations.avalara;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using Marketplace.Models;
using ordercloud.integrations.library;

namespace Marketplace.Common.Commands
{
    public interface IResaleCertCommand
    {
        Task<TaxCertificate> GetAsync(int companyID, string locationID, VerifiedUserContext verifiedUser);
        Task<TaxCertificate> CreateAsync(int companyID, string locationID, TaxCertificate cert, VerifiedUserContext verifiedUser);
        Task<TaxCertificate> UpdateAsync(int companyID, string locationID, TaxCertificate cert, VerifiedUserContext verifiedUser);
    }

    public class ResaleCertCommand : IResaleCertCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly IAvalaraCommand _avalara;
        private readonly ILocationPermissionCommand _locationPermissionCommand;

        public ResaleCertCommand(IAvalaraCommand avalara, IOrderCloudClient oc, ILocationPermissionCommand locationPermissionCommand)
        {
			_oc = oc;
            _avalara = avalara;
            _locationPermissionCommand = locationPermissionCommand;
        }

        public async Task<TaxCertificate> GetAsync(int companyID, string locationID, VerifiedUserContext verifiedUser)
        {
            await EnsureUserCanManageLocationResaleCert(locationID, verifiedUser);
            var buyerID = locationID.Split('-')[0];
            var address = await _oc.Addresses.GetAsync<MarketplaceAddressBuyer>(buyerID, locationID);
            if(address.xp.AvalaraCertificateID != null)
            {
                return await _avalara.GetCertificateAsync(companyID, (int)address.xp.AvalaraCertificateID);
            } else
            {
                return new TaxCertificate();
            }
        }

        public async Task<TaxCertificate> CreateAsync(int companyID, string locationID, TaxCertificate cert, VerifiedUserContext verifiedUser)
        {
            await EnsureUserCanManageLocationResaleCert(locationID, verifiedUser);
            var buyerID = locationID.Split('-')[0];
            var createdCert = await _avalara.CreateCertificateAsync(companyID, cert);
            var newAddressXP = new
            {
                AvalaraCertificateID = createdCert.ID,
                AvalaraCertificateExpiration = createdCert.ExpirationDate
            };
            var addressPatch = new PartialAddress
            {
                xp = newAddressXP
            };
            var address = await _oc.Addresses.PatchAsync(buyerID, locationID, addressPatch);
            return createdCert;
        }

        public async Task<TaxCertificate> UpdateAsync(int companyID, string locationID, TaxCertificate cert, VerifiedUserContext verifiedUser)
        {
            await EnsureUserCanManageLocationResaleCert(locationID, verifiedUser);
            await EnsureCertIDMatchesAddressXPForAuthenticatedLocation(locationID, cert);
            var updatedCert = await _avalara.UpdateCertificateAsync(companyID, cert.ID, cert);
            return updatedCert;
        }

        private async Task EnsureCertIDMatchesAddressXPForAuthenticatedLocation(string locationID, TaxCertificate cert)
        {
            // ensures that someone can only modify the cert they have access to
            var buyerID = locationID.Split('-')[0];
            var address = await _oc.Addresses.GetAsync<MarketplaceAddressBuyer>(buyerID, locationID);
            var certificateIDPerAddressXP = address.xp.AvalaraCertificateID;
            Require.That(certificateIDPerAddressXP == cert.ID, new ErrorCode("Insufficient Access", 403, $"User cannot modofiy this cert"));
        }

        private async Task EnsureUserCanManageLocationResaleCert(string locationID, VerifiedUserContext verifiedUser)
        {
            var hasAccess = await _locationPermissionCommand.IsUserInAccessGroup(locationID, UserGroupSuffix.ResaleCertAdmin.ToString(), verifiedUser);
            Require.That(hasAccess, new ErrorCode("Insufficient Access", 403, $"User cannot manage resale certs for: {locationID}"));
        }


    };
}