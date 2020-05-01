using Marketplace.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Models.Misc;

namespace Marketplace.Common.TemporaryAppConstants
{
    public static class SEBUserTypes
    {
        public static List<MarketplaceUserType> Supplier() {
            return new List<MarketplaceUserType>()
            {
                new MarketplaceUserType {
                    UserGroupName = "Order Admin",
                    UserGroupType = UserGroupType.UserPermissions,
                    UserGroupIDSuffix = "OrderAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPOrderAdmin,
                        CustomRole.MPShipmentAdmin,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Account Admin",
                    UserGroupType = UserGroupType.UserPermissions,
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
                    UserGroupType = UserGroupType.UserPermissions,
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
                    UserGroupType = UserGroupType.UserPermissions,
                    UserGroupIDSuffix = "ApprovalRuleAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPApprovalRuleAdmin,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Credit Card Admin",
                    UserGroupType = UserGroupType.UserPermissions,
                    UserGroupIDSuffix = "CreditCardAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPCreditCardAdmin,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Address Admin",
                    UserGroupType = UserGroupType.UserPermissions,
                    UserGroupIDSuffix = "AddressAdmin",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPAddressAdmin,
                    }
                },
            };
        }
        public static List<MarketplaceUserType> BuyerLocation()
        {
            return new List<MarketplaceUserType>()
            {
                new MarketplaceUserType {
                    UserGroupName = "Order Approver",
                    UserGroupType = UserGroupType.Approval,
                    UserGroupIDSuffix = "OrderApprover",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPOrderApprover,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Needs Approval",
                    UserGroupType = UserGroupType.Approval,
                    UserGroupIDSuffix = "NeedsApproval",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPNeedsApproval,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "View All Location Orders",
                    UserGroupType = UserGroupType.UserPermissions,
                    UserGroupIDSuffix = "ViewAllLocationOrders",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPViewAllLocationOrders,
                    }
                }
            };
        }
    }
}
