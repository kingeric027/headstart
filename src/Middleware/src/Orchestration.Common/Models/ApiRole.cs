using System;
using System.Collections.Generic;
using System.Text;

namespace Orchestration.Common.Models
{
    public enum ApiRole
    {
        //[DocIgnore]
        GrantForAnyRole,
        AddressAdmin, AddressReader, AdminAddressAdmin, AdminAddressReader, AdminUserAdmin, AdminUserGroupAdmin, AdminUserGroupReader, AdminUserReader, ApiClientAdmin, ApiClientReader,
        ApprovalRuleAdmin, ApprovalRuleReader, BuyerAdmin, BuyerImpersonation, BuyerReader, BuyerUserAdmin, BuyerUserReader, CatalogAdmin, CatalogReader,
        CategoryAdmin, CategoryReader, CostCenterAdmin, CostCenterReader, CreditCardAdmin, CreditCardReader, FullAccess, IncrementorAdmin, IncrementorReader,
        InventoryAdmin, MeAddressAdmin, MeAdmin, MeCreditCardAdmin, MessageConfigAssignmentAdmin, MessageSenderAdmin, MessageSenderReader, MeXpAdmin, OrderAdmin, OrderReader, OverrideShipping,
        OverrideTax, OverrideUnitPrice, PasswordReset, PriceScheduleAdmin, PriceScheduleReader, ProductAdmin, ProductAssignmentAdmin, ProductFacetAdmin,
        ProductFacetReader, ProductReader, PromotionAdmin, PromotionReader, SetSecurityProfile, ShipmentAdmin, ShipmentReader, Shopper, SpendingAccountAdmin,
        SpendingAccountReader, SupplierAddressAdmin, SupplierAddressReader, SupplierAdmin, SupplierReader, SupplierUserAdmin, SupplierUserGroupAdmin, SupplierUserGroupReader,
        SupplierUserReader, UnsubmittedOrderReader, UserGroupAdmin, UserGroupReader
    }
}
