using Marketplace.Common.Models;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Models;
using ErrorCodes = Marketplace.Helpers.Exceptions.ErrorCodes;

namespace Marketplace.Common.Extensions
{
    public static class OrchestrationExtensions
    {
        public static string BuildPath<T>(this OrchestrationObject<T> obj, string resourceId) where T : IMarketplaceObject
        {
            return $"{resourceId}/{obj.Model.Type()}/{obj.ID}".ToLower();
        }

        public static RecordType Type(this IMarketplaceObject obj)
        {
            switch (obj.GetType().Name)
            {
                case nameof(MarketplaceProduct):
                    return RecordType.Product;
                case nameof(MarketplaceProductFacet):
                    return RecordType.ProductFacet;
                case nameof(MarketplacePriceSchedule):
                    return RecordType.PriceSchedule;
                case nameof(MarketplaceSpec):
                    return RecordType.Spec;
                case nameof(MarketplaceSpecOption):
                    return RecordType.SpecOption;
                case nameof(MarketplaceSpecProductAssignment):
                    return RecordType.SpecProductAssignment;
                case nameof(MarketplaceBuyer):
                    return RecordType.Buyer;
                case nameof(MarketplaceAddress):
                    return RecordType.Address;
                case nameof(BaseCostCenter):
                    return RecordType.CostCenter;
                case nameof(MarketplaceUser):
                    return RecordType.User;
                case nameof(MarketplaceUserGroup):
                    return RecordType.UserGroup;
                case nameof(MarketplaceUserGroupAssignment):
                    return RecordType.UserGroupAssignment;
                case nameof(MarketplaceAddressAssignment):
                    return RecordType.AddressAssignment;
                case nameof(MarketplaceCatalogAssignment):
                    return RecordType.CatalogAssignment;
                case nameof(MarketplaceCatalog):
                    return RecordType.Catalog;
                default:
                    throw new ApiErrorException(ErrorCodes.All["UnrecognizedType"], obj);
            }
        }
    }
}
