namespace Marketplace.Models.Misc
{
	public enum CustomRole
	{
		// seller/supplier
		MPMeProductAdmin, 
		MPMeProductReader,
		MPProductAdmin,
		MeProductAdmin,
		MPProductReader,
		MPPromotionReader,
		MPPromotionAdmin,
		MPCategoryAdmin,
		MPCategoryReader, 
		MPOrderAdmin,
		MPOrderReader,
		MPShipmentAdmin,
		MPBuyerAdmin, 
		MPBuyerReader,
		MPSellerAdmin,
		MPSupplierAdmin, 
		MPMeSupplierAddressAdmin,
		MPMeSupplierUserAdmin,
		MPSupplierUserGroupAdmin,
		MPReportReader,

		// buyer
		MPBaseBuyer,
		MPApprovalRuleAdmin,
		MPCreditCardAdmin,
		MPAddressAdmin,
		MPOrderApprover,
		MPNeedsApproval,
		MPViewAllLocationOrders
	}
}
