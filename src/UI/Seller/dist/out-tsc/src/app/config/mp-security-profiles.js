var MPMeProductAdmin = {
    // Assigned to user types who want to manage own products in OC
    RoleName: 'MPMeProductAdmin',
    OrderCloudRoles: ['ProductAdmin', 'PriceScheduleAdmin', 'InventoryAdmin'],
};
var MPMeProductReader = {
    // Assigned to user types who want to view own products in OC
    RoleName: 'MPMeProductReader',
    OrderCloudRoles: ['ProductReader', 'PriceScheduleReader', 'InventoryReader'],
};
var MPProductAdmin = {
    // Assigned to user types who want to manager the display to buyers of others products in OC
    RoleName: 'MPProductAdmin',
    OrderCloudRoles: ['ProductReader', 'CatalogAdmin', 'ProductAssignmentAdmin', 'ProductFacetAdmin'],
};
var MPProductReader = {
    // Assigned to user types who want to view the display to buyers of others products in OC but cannot manager (might not be needed for SEB)
    RoleName: 'MPProductReader',
    OrderCloudRoles: ['ProductReader', 'CatalogReader', 'ProductFacetReader'],
};
var MPPromotionAdmin = {
    // Assigned to user types who want to administer promotions
    RoleName: 'MPPromotionAdmin',
    OrderCloudRoles: ['PromotionAdmin'],
};
var MPPromotionReader = {
    // Assigned to user types who want to view promotions
    RoleName: 'MPPromotionReader',
    OrderCloudRoles: ['PromotionReader'],
};
var MPCategoryAdmin = {
    // Assigned to user types who want to administer categorys and assignments
    RoleName: 'MPCategoryAdmin',
    OrderCloudRoles: ['CategoryAdmin'],
};
var MPCategoryReader = {
    // Assigned to user types who want to view categorys
    RoleName: 'MPCategoryReader',
    OrderCloudRoles: ['CategoryReader'],
};
var MPOrderAdmin = {
    // Assigned to user types who want to edit orders, line items, and shipments. Would likely by a supplier who needs to make manual updates to an order
    RoleName: 'MPOrderAdmin',
    OrderCloudRoles: ['OrderAdmin', 'ShipmentReader'],
};
var MPOrderReader = {
    // Assigned to a user type who wants to view orders. Would likely be a seller user who shouldn't edit orders but wants to view
    RoleName: 'MPOrderReader',
    OrderCloudRoles: ['OrderReader', 'ShipmentReader'],
};
var MPShipmentAdmin = {
    // Assigned to a user type who wants to administer shipping for a supplier
    RoleName: 'MPShipmentAdmin',
    OrderCloudRoles: ['OrderReader', 'ShipmentAdmin'],
};
// unclear if we need a MeBuyerAdmin
// will need to be some disucssion about the breakout of these for SEB
var MPBuyerAdmin = {
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
var MPBuyerReader = {
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
var MPSellerAdmin = {
    // Assigned to a user type who wants to view buyers and related subresources
    RoleName: 'MPSellerAdmin',
    OrderCloudRoles: ['AdminUserAdmin'],
};
var MPSupplierAdmin = {
    // Assigned to a user type who wants to view buyers and related subresources
    RoleName: 'MPSupplierAdmin',
    OrderCloudRoles: ['SupplierAdmin', 'SupplierUserAdmin', 'SupplierAddressAdmin'],
};
var MPMeSupplierAdmin = {
    RoleName: 'MPMeSupplierAdmin',
    OrderCloudRoles: ['SupplierAdmin', 'SupplierAddressReader', 'SupplierUserReader'],
};
var MPMeSupplierAddressAdmin = {
    RoleName: 'MPMeSupplierAddressAdmin',
    OrderCloudRoles: ['SupplierReader', 'SupplierAddressAdmin'],
};
var MPMeSupplierUserAdmin = {
    RoleName: 'MPMeSupplierUserAdmin',
    OrderCloudRoles: ['SupplierReader', 'SupplierUserAdmin'],
};
var MPReportReader = {
    RoleName: 'MPReportReader',
    OrderCloudRoles: [],
};
var AllMPRoles = [
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
var MarketPlaceManager = {
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
var SupplierManager = {
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
var SupplierTeamMember = {
    Name: 'SupplierTeamMember',
    MPRoles: [MPMeProductAdmin, MPOrderAdmin, MPShipmentAdmin, MPMeSupplierAdmin, MPMeSupplierAddressAdmin],
};
var SEBUserTypes = [SupplierManager, SupplierTeamMember, MarketPlaceManager];
//# sourceMappingURL=mp-security-profiles.js.map