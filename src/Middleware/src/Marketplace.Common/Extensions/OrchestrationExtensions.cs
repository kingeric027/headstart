﻿using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Models;
using ordercloud.integrations.library;
using ErrorCodes = Marketplace.Models.ErrorCodes;

namespace Marketplace.Common.Extensions
{
    public static class OrchestrationExtensions
    {
        public static string BuildPath<T>(this OrchestrationObject<T> obj, string resourceId, string clientId) where T : IHSObject
        {
            return $"{resourceId}/{clientId}/{obj.Model.Type()}/{obj.ID}".ToLower();
        }

        public static RecordType Type(this IHSObject obj)
        {
            switch (obj.GetType().Name)
            {
                case nameof(SuperMarketplaceProduct):
                    return RecordType.HydratedProduct;
                case nameof(HSProduct):
                    return RecordType.Product;
                case nameof(HSProductFacet):
                    return RecordType.ProductFacet;
                case nameof(HSPriceSchedule):
                    return RecordType.PriceSchedule;
                case nameof(HSSpec):
                    return RecordType.Spec;
                case nameof(HSSpecOption):
                    return RecordType.SpecOption;
                case nameof(HSSpecProductAssignment):
                    return RecordType.SpecProductAssignment;
                case nameof(HSBuyer):
                    return RecordType.Buyer;
                case nameof(HSAddressBuyer):
                    return RecordType.Address;
                case nameof(HSCostCenter):
                    return RecordType.CostCenter;
                case nameof(HSUser):
                    return RecordType.User;
                case nameof(HSUserGroup):
                    return RecordType.UserGroup;
                case nameof(HSUserGroupAssignment):
                    return RecordType.UserGroupAssignment;
                case nameof(HSAddressAssignment):
                    return RecordType.AddressAssignment;
                case nameof(HSCatalogAssignment):
                    return RecordType.CatalogAssignment;
                case nameof(HSCatalog):
                    return RecordType.Catalog;
                case nameof(TemplateProductFlat):
                    return RecordType.TemplateProductFlat;
                default:
                    throw new OrderCloudIntegrationException(ErrorCodes.All["UnrecognizedType"], obj);
            }
        }
    }
}
