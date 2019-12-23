﻿using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Models;
using ErrorCodes = Marketplace.Helpers.Exceptions.ErrorCodes;

namespace Marketplace.Common.Extensions
{
    public static class OrchestrationExtensions
    {
        public static string BuildPath(this IOrchestrationObject obj, string resourceId)
        {
            return $"{resourceId}/{obj.Type()}/{obj.ID}".ToLower();
        }

        public static RecordType Type(this IOrchestrationObject obj)
        {
            switch (obj.GetType().Name)
            {
                case nameof(OrchestrationProduct):
                    return RecordType.Product;
                case nameof(OrchestrationProductFacet):
                    return RecordType.ProductFacet;
                case nameof(OrchestrationPriceSchedule):
                    return RecordType.PriceSchedule;
                case nameof(OrchestrationSpec):
                    return RecordType.Spec;
                case nameof(OrchestrationSpecOption):
                    return RecordType.SpecOption;
                case nameof(OrchestrationSpecProductAssignment):
                    return RecordType.SpecProductAssignment;
                case nameof(OrchestrationBuyer):
                    return RecordType.Buyer;
                case nameof(OrchestrationAddress):
                    return RecordType.Address;
                case nameof(OrchestrationCostCenter):
                    return RecordType.CostCenter;
                case nameof(OrchestrationUser):
                    return RecordType.User;
                case nameof(OrchestrationUserGroup):
                    return RecordType.UserGroup;
                case nameof(OrchestrationUserGroupAssignment):
                    return RecordType.UserGroupAssignment;
                case nameof(OrchestrationAddressAssignment):
                    return RecordType.AddressAssignment;
                case nameof(OrchestrationCatalogAssignment):
                    return RecordType.CatalogAssignment;
                default:
                    throw new ApiErrorException(ErrorCodes.All["UnrecognizedType"], obj);
            }
        }
    }
}
