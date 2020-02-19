/*
* Marketplace has distinct roles which are sometimes a combination of OrderCloud roles or sometimes a single OrderCloud role
* We have choosen to represent these MP roles with security profiles and identifying custom roles for example: MPProductAdmin (OrderCloud roles: ProductAdmin, FacetAdmin, SpecAdmin)
*
*/
interface MPRole {
  RoleName: string;
  OrderCloudRoles: string[];
}
const MPMeProductAdmin: MPRole = {
  // Assigned to user types who want to manage own products in OC
  RoleName: 'MPMeProductAdmin',
  OrderCloudRoles: ['ProductAdmin', 'PriceScheduleAdmin', 'InventoryAdmin'],
};
const MPMeProductReader: MPRole = {
  // Assigned to user types who want to view own products in OC
  RoleName: 'MPMeProductReader',
  OrderCloudRoles: ['ProductReader', 'PriceScheduleReader', 'InventoryReader'],
};
const MPProductAdmin: MPRole = {
  // Assigned to user types who want to manager the display to buyers of others products in OC
  RoleName: 'MPProductAdmin',
  OrderCloudRoles: ['ProductReader', 'CatalogAdmin', 'ProductAssignmentAdmin', 'ProductFacetAdmin'],
};
const MPProductReader: MPRole = {
  // Assigned to user types who want to view the display to buyers of others products in OC but cannot manager (might not be needed for SEB)
  RoleName: 'MPProductReader',
  OrderCloudRoles: ['ProductReader', 'CatalogReader', 'ProductFacetReader'],
};
const MPPromotionAdmin: MPRole = {
  // Assigned to user types who want to administer promotions
  RoleName: 'MPPromotionAdmin',
  OrderCloudRoles: ['PromotionAdmin'],
};
const MPPromotionReader: MPRole = {
  // Assigned to user types who want to view promotions
  RoleName: 'MPPromotionReader',
  OrderCloudRoles: ['PromotionReader'],
};
const MPCategoryAdmin: MPRole = {
  // Assigned to user types who want to administer categorys and assignments
  RoleName: 'MPCategoryAdmin',
  OrderCloudRoles: ['CategoryAdmin'],
};
const MPCategoryReader: MPRole = {
  // Assigned to user types who want to view categorys
  RoleName: 'MPCategoryReader',
  OrderCloudRoles: ['CategoryReader'],
};
const MPOrderAdmin: MPRole = {
  // Assigned to user types who want to edit orders, line items, and shipments. Would likely by a supplier who needs to make manual updates to an order
  RoleName: 'MPOrderAdmin',
  OrderCloudRoles: ['OrderAdmin', 'ShipmentReader'],
};
const MPOrderReader: MPRole = {
  // Assigned to a user type who wants to view orders. Would likely be a seller user who shouldn't edit orders but wants to view
  RoleName: 'MPOrderReader',
  OrderCloudRoles: ['OrderReader', 'ShipmentReader'],
};
const MPShipmentAdmin: MPRole = {
  // Assigned to a user type who wants to administer shipping for a supplier
  RoleName: 'MPShipmentAdmin',
  OrderCloudRoles: ['OrderReader', 'ShipmentAdmin'],
};
// unclear if we need a MeBuyerAdmin
// will need to be some disucssion about the breakout of these for SEB
const MPBuyerAdmin: MPRole = {
  // Assigned to a user type who wants to administer buyers and related subresources
  RoleName: 'MPBuyerAdmin',
  OrderCloudRoles: [
    'BuyerAdmin',
    'BuyerUserAdmin',
    'UserGroupAdmin',
    'AddressAdmin',
    'CreditCardAdmin',
    'ApprovalRuleAdmin',
  ],
};
const MPBuyerReader: MPRole = {
  // Assigned to a user type who wants to view buyers and related subresources
  RoleName: 'MPBuyerReader',
  OrderCloudRoles: [
    'BuyerReader',
    'BuyerUserReader',
    'UserGroupReader',
    'AddressReader',
    'CreditCardReader',
    'ApprovalRuleReader',
  ],
};
const MPSellerAdmin: MPRole = {
  // Assigned to a user type who wants to view buyers and related subresources
  RoleName: 'MPSellerAdmin',
  OrderCloudRoles: ['AdminUserAdmin'],
};
const MPSupplierAdmin: MPRole = {
  // Assigned to a user type who wants to view buyers and related subresources
  RoleName: 'MPSupplierAdmin',
  OrderCloudRoles: ['SupplierAdmin', 'SupplierUserAdmin', 'SupplierAddressAdmin'],
};
const MPMeSupplierAdmin: MPRole = {
  RoleName: 'MPMeSupplierAdmin',
  OrderCloudRoles: ['SupplierAdmin', 'SupplierAddressReader', 'SupplierUserReader'],
};
const MPMeSupplierAddressAdmin: MPRole = {
  RoleName: 'MPMeSupplierAddressAdmin',
  OrderCloudRoles: ['SupplierReader', 'SupplierAddressAdmin'],
};
const MPMeSupplierUserAdmin: MPRole = {
  RoleName: 'MPMeSupplierUserAdmin',
  OrderCloudRoles: ['SupplierReader', 'SupplierUserAdmin'],
};
const MPReportReader: MPRole = {
  RoleName: 'MPReportReader',
  OrderCloudRoles: [],
};
const AllMPRoles = [
  MPMeProductAdmin,
  MPMeProductReader,
  MPProductAdmin,
  MPProductReader,
  MPPromotionAdmin,
  MPPromotionReader,
  MPCategoryAdmin,
  MPCategoryReader,
  MPOrderAdmin,
  MPOrderReader,
  MPShipmentAdmin,
  MPBuyerAdmin,
  MPBuyerReader,
  MPSellerAdmin,
  MPSupplierAdmin,
  MPMeSupplierAdmin,
  MPMeSupplierAddressAdmin,
  MPMeSupplierUserAdmin,
  MPReportReader,
];
interface UserType {
  Name: string;
  MPRoles: MPRole[];
}
const MarketPlaceManager = {
  Name: 'MarketPlaceManager',
  MPRoles: [
    MPProductReader,
    MPPromotionReader,
    MPCategoryReader,
    MPOrderReader,
    MPBuyerReader,
    MPSellerAdmin,
    MPSupplierAdmin,
    MPMeSupplierAdmin,
  ],
};

// SEB Specific Roles
// Ultimately these will not be hardcoded in the app but live outside and by dynamic

const SupplierManager = {
  Name: 'SupplierManager',
  MPRoles: [
    MPMeProductAdmin,
    MPCategoryReader,
    MPOrderAdmin,
    MPShipmentAdmin,
    MPMeSupplierAdmin,
    MPMeSupplierAddressAdmin,
    MPMeSupplierUserAdmin,
  ],
};
const SupplierTeamMember = {
  Name: 'SupplierTeamMember',
  MPRoles: [MPMeProductAdmin, MPOrderAdmin, MPShipmentAdmin, MPMeSupplierAdmin, MPMeSupplierAddressAdmin],
};
const SEBUserTypes = [SupplierManager, SupplierTeamMember, MarketPlaceManager];
