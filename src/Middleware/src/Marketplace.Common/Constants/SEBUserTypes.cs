using Marketplace.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Models.Misc;

namespace Marketplace.Common.Constants
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
                        CustomRole.MPSupplierUserGroupAdmin,
                        CustomRole.MPMeSupplierAdmin
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
                 new MarketplaceUserType {
                    UserGroupName = "Report Reader",
                    UserGroupType = UserGroupType.UserPermissions,
                    UserGroupIDSuffix = "ReportReader",
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPReportReader,
                    }
                },
            };
                
        }

        public static List<MarketplaceUserType> BuyerLocation()
        {
            return new List<MarketplaceUserType>()
            {
                 new MarketplaceUserType {
                    UserGroupName = "Location Permission Admin",
                    UserGroupType = UserGroupType.LocationPermissions,
                    UserGroupIDSuffix = UserGroupSuffix.PermissionAdmin.ToString(),
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPLocationPermissionAdmin,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Location Order Approver",
                    UserGroupType = UserGroupType.LocationPermissions,
                    UserGroupIDSuffix = UserGroupSuffix.OrderApprover.ToString(),
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPLocationOrderApprover,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Location Needs Approval",
                    UserGroupType = UserGroupType.LocationPermissions,
                    UserGroupIDSuffix = UserGroupSuffix.NeedsApproval.ToString(),
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPLocationNeedsApproval,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "View All Location Orders",
                    UserGroupType = UserGroupType.LocationPermissions,
                    UserGroupIDSuffix = UserGroupSuffix.ViewAllOrders.ToString(),
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPLocationViewAllOrders,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Credit Card Admin",
                    UserGroupType = UserGroupType.LocationPermissions,
                    UserGroupIDSuffix = UserGroupSuffix.CreditCardAdmin.ToString(),
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPLocationCreditCardAdmin,
                    }
                },
                new MarketplaceUserType {
                    UserGroupName = "Address Admin",
                    UserGroupType = UserGroupType.LocationPermissions,
                    UserGroupIDSuffix = UserGroupSuffix.AddressAdmin.ToString(),
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPLocationAddressAdmin,
                    }
                },
                   new MarketplaceUserType {
                    UserGroupName = "Resale Cert Admin",
                    UserGroupType = UserGroupType.LocationPermissions,
                    UserGroupIDSuffix = UserGroupSuffix.ResaleCertAdmin.ToString(),
                    CustomRoles = new List<CustomRole>
                    {
                        CustomRole.MPLocationResaleCertAdmin,
                    }
                },
            };
        }
    }
}
