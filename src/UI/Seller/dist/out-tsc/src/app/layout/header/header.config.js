import { SELLER, SUPPLIER } from '@app-seller/shared/models/ordercloud-user.types';
// ! included to ensure no overlap with ordercloud ids as this in invalid in ids
export var REDIRECT_TO_FIRST_PARENT = '!';
// Products
var AllProducts = {
    rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
    title: 'All Products',
    route: '/products',
};
var LiveProducts = {
    rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
    title: 'Live Products',
    route: '/products',
    queryParams: { 'xp.Status': 'Published' },
};
var PendingProducts = {
    rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
    title: 'Pending Products',
    route: '/products',
    queryParams: { 'xp.Status': 'Draft' },
};
var Promotions = {
    rolesWithAccess: ['MPPromotionAdmin', 'MPPromotionReader'],
    title: 'Promotions',
    route: '/promotions',
};
var Categories = {
    rolesWithAccess: ['MPCategoryAdmin', 'MPCategoryReader'],
    title: 'Categories',
    route: '/categories',
};
var ProductNavGrouping = {
    rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
    title: 'Products',
    route: '/products',
    subRoutes: [AllProducts, LiveProducts, PendingProducts, Promotions, Categories],
};
// Orders
var BuyerOrders = {
    rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdmin'],
    title: 'Incoming Buyer Orders',
    route: '/orders',
    queryParams: { OrderDirection: 'Incoming' },
};
var SupplierPurchaseOrders = {
    rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdmin'],
    title: 'Outgoing Supplier Orders',
    route: '/orders',
    queryParams: { OrderDirection: 'Outgoing' },
};
var Orders = {
    rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
    title: 'Orders',
    route: '/orders',
};
var AwaitingApprovalOrders = {
    rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
    title: 'Awaiting Approval Orders',
    route: '/orders',
};
var ShippedOrders = {
    rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
    title: 'Shipped Orders',
    route: '/orders',
};
var CancelledOrders = {
    rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
    title: 'Cancelled Orders',
    route: '/orders',
};
var SellerOrderNavGrouping = {
    rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
    title: 'Orders',
    route: '/orders',
    orderCloudUserTypesWithAccess: [SELLER],
    subRoutes: [BuyerOrders, SupplierPurchaseOrders],
};
var SupplierOrderNavGrouping = {
    rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
    title: 'Orders',
    route: '/orders',
    orderCloudUserTypesWithAccess: [SUPPLIER],
    subRoutes: [Orders, AwaitingApprovalOrders, ShippedOrders, CancelledOrders],
};
// Buyers
var AllBuyers = {
    rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
    title: 'All Buyers',
    route: '/buyers',
};
var BuyerUsers = {
    rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
    title: 'Users',
    route: "/buyers/" + REDIRECT_TO_FIRST_PARENT + "/users",
};
var BuyerPurchasingLocations = {
    rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
    title: 'Purchasing Locations',
    route: "/buyers/" + REDIRECT_TO_FIRST_PARENT + "/locations",
};
var BuyerPaymentMethods = {
    rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
    title: 'Payment Methods',
    route: "/buyers/" + REDIRECT_TO_FIRST_PARENT + "/payments",
};
var BuyerApprovalRules = {
    rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
    title: 'Approval Rules',
    route: "/buyers/" + REDIRECT_TO_FIRST_PARENT + "/approvals",
};
var BuyerNavGrouping = {
    rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
    title: 'Buyers',
    route: '/buyers',
    subRoutes: [AllBuyers, BuyerUsers, BuyerPurchasingLocations, BuyerPaymentMethods, BuyerApprovalRules],
};
// Suppliers
var AllSuppliers = {
    rolesWithAccess: ['MPSupplierAdmin'],
    title: 'All Suppliers',
    route: '/suppliers',
};
var SupplierUsers = {
    rolesWithAccess: ['MPSupplierAdmin'],
    title: 'Users',
    route: "/suppliers/" + REDIRECT_TO_FIRST_PARENT + "/users",
};
var SupplierLocations = {
    rolesWithAccess: ['MPSupplierAdmin', 'MPMeSupplierAdmin'],
    title: 'Locations',
    route: "/suppliers/" + REDIRECT_TO_FIRST_PARENT + "/locations",
};
var SupplierNavGrouping = {
    rolesWithAccess: ['MPSupplierAdmin'],
    title: 'Suppliers',
    route: '/suppliers',
    subRoutes: [AllSuppliers, SupplierUsers, SupplierLocations],
};
var OrchestionLogs = {
    rolesWithAccess: ['MPReportReader'],
    title: 'Orchestion Logs',
    route: 'reports/orchestration-logs',
};
var ReportsNavGrouping = {
    rolesWithAccess: ['MPReportReader'],
    title: 'Reports',
    route: '/reports',
    subRoutes: [OrchestionLogs],
};
var SellerUsers = {
    rolesWithAccess: ['MPSellerAdmin'],
    title: 'Seller Users',
    route: '/seller-users',
};
var Storefronts = {
    rolesWithAccess: ['MPStoreFrontAdmin'],
    title: 'StoreFronts',
    route: '/storefronts',
};
var PublicProfile = {
    rolesWithAccess: ['MPMeSupplierAdmin'],
    title: 'Public Profile',
    route: '/my-supplier-profile',
};
var AllNavGroupings = [
    ProductNavGrouping,
    SupplierOrderNavGrouping,
    SellerOrderNavGrouping,
    BuyerNavGrouping,
    SupplierNavGrouping,
    ReportsNavGrouping,
    SellerUsers,
    Storefronts,
    PublicProfile,
];
export var getHeaderConfig = function (userRoles, orderCloudUserType) {
    var navGroupingsApplicableToUser = filterOutNavGroupings(AllNavGroupings, userRoles, orderCloudUserType);
    return navGroupingsApplicableToUser.map(function (navGrouping) {
        if (!navGrouping.subRoutes) {
            return navGrouping;
        }
        else {
            var routesApplicableToUser = filterOutNavGroupings(navGrouping.subRoutes, userRoles, orderCloudUserType);
            navGrouping.subRoutes = routesApplicableToUser;
            return navGrouping;
        }
    });
};
var filterOutNavGroupings = function (navGroupings, userRoles, orderCloudUserType) {
    return navGroupings.filter(function (navGrouping) {
        return (navGrouping.rolesWithAccess.some(function (role) { return userRoles.includes(role); }) &&
            (!navGrouping.orderCloudUserTypesWithAccess ||
                navGrouping.orderCloudUserTypesWithAccess.includes(orderCloudUserType)));
    });
};
//# sourceMappingURL=header.config.js.map