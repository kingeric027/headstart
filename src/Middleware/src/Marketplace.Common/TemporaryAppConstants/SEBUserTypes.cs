﻿using Marketplace.Common.Models;
using Marketplace.Models.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.TemporaryAppConstants
{
    public static class SEBUserTypes
    {
        public static List<MarketplaceUserType> Supplier() {
            return new List<MarketplaceUserType>()
            {
                new MarketplaceUserType {
                    UserGroupName = "Order Admin",
                    UserGroupIDSuffix = "OrderAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPOrderAdmin,
                        CustomRole.MPShipmentAdmin,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Account Admin",
                    UserGroupIDSuffix = "AccountAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPMeSupplierAddressAdmin,
                        CustomRole.MPMeSupplierUserAdmin,
                        CustomRole.MPSupplierUserGroupAdmin
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Product Admin",
                    UserGroupIDSuffix = "ProductAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPMeProductAdmin,
                    }
                },
            };
                
        }

        public static List<MarketplaceUserType> Buyer()
        {
            return new List<MarketplaceUserType>()
            {
                new MarketplaceUserType {
                    UserGroupName = "Approval Rule Admin",
                    UserGroupIDSuffix = "ApprovalRuleAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPApprovalRuleAdmin,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Credit Card Admin",
                    UserGroupIDSuffix = "CreditCardAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPCreditCardAdmin,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Address Admin",
                    UserGroupIDSuffix = "AddressAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPAddressAdmin,
                    }
                },
            };

        }
    }
}