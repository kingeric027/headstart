import { SELLER, SUPPLIER } from '@app-seller/shared/models/ordercloud-user.types';

// ! included to ensure no overlap with ordercloud ids as this in invalid in ids
export const REDIRECT_TO_FIRST_PARENT = '!';

export interface MPRoute {
  rolesWithAccess: string[];
  // this allows the routes to be narrowed based upon OC user type
  orderCloudUserTypesWithAccess?: string[];
  title: string;
  route: string;
  queryParams?: Record<string, any>;

  // if subroutes are included, itesms will display in a dropdown
  subRoutes?: MPRoute[];
}

// Products
const AllProducts: MPRoute = {
  rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'All Products',
  route: '/products',
};

const LiveProducts: MPRoute = {
  rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'Live Products',
  route: '/products',
  queryParams: { 'xp.Status': 'Published' },
};

const PendingProducts: MPRoute = {
  rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'Pending Products',
  route: '/products',
  queryParams: { 'xp.Status': 'Draft' },
};

const Promotions: MPRoute = {
  rolesWithAccess: ['MPPromotionAdmin', 'MPPromotionReader'],
  title: 'Promotions',
  route: '/promotions',
};

const ProductNavGrouping: MPRoute = {
  rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'Products',
  route: '/products',
  subRoutes: [AllProducts, LiveProducts, PendingProducts, Promotions],
};

// Orders
const BuyerOrders: MPRoute = {
  rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdmin'],
  title: 'Incoming Buyer Orders',
  route: '/orders',
  queryParams: { OrderDirection: 'Incoming' },
};

const SupplierPurchaseOrders: MPRoute = {
  rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdmin'],
  title: 'Outgoing Supplier Orders',
  route: '/orders',
  queryParams: { OrderDirection: 'Outgoing' },
};

const Orders: MPRoute = {
  rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Orders',
  route: '/orders',
};

const AwaitingApprovalOrders: MPRoute = {
  rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Awaiting Approval Orders',
  route: '/orders',
};

const ShippedOrders: MPRoute = {
  rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Shipped Orders',
  route: '/orders',
};

const CancelledOrders: MPRoute = {
  rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Cancelled Orders',
  route: '/orders',
};

const SellerOrderNavGrouping: MPRoute = {
  rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Orders',
  route: '/orders',
  orderCloudUserTypesWithAccess: [SELLER],
  subRoutes: [BuyerOrders, SupplierPurchaseOrders],
};

const SupplierOrderNavGrouping: MPRoute = {
  rolesWithAccess: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Orders',
  route: '/orders',
  orderCloudUserTypesWithAccess: [SUPPLIER],
  subRoutes: [Orders, AwaitingApprovalOrders, ShippedOrders, CancelledOrders],
};

// Buyers
const AllBuyers: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'All Buyers',
  route: '/buyers',
};

const BuyerUsers: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Users',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const BuyerPurchasingLocations: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Locations',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/locations`,
};

const BuyerPaymentMethods: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Payment Methods',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/payments`,
};

const BuyerApprovalRules: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Approval Rules',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/approvals`,
};

const BuyerCategories: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Categories',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/categories`,
};

const BuyerNavGrouping = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Buyers',
  route: '/buyers',
  subRoutes: [
    AllBuyers,
    BuyerUsers,
    BuyerPurchasingLocations,
    BuyerPaymentMethods,
    BuyerApprovalRules,
    BuyerCategories,
  ],
};

// Suppliers
const AllSuppliers: MPRoute = {
  rolesWithAccess: ['MPSupplierAdmin'],
  title: 'All Suppliers',
  route: '/suppliers',
};

const SupplierUsers: MPRoute = {
  rolesWithAccess: ['MPSupplierAdmin'],
  title: 'Users',
  route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const SupplierLocations: MPRoute = {
  rolesWithAccess: ['MPSupplierAdmin', 'MPMeSupplierAdmin'],
  title: 'Locations',
  route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/locations`,
};

const SupplierNavGrouping: MPRoute = {
  rolesWithAccess: ['MPSupplierAdmin'],
  title: 'Suppliers',
  route: '/suppliers',
  subRoutes: [AllSuppliers, SupplierUsers, SupplierLocations],
};

const OrchestrationLogs = {
  rolesWithAccess: ['MPReportReader'],
  title: 'Orchestration Logs',
  route: 'reports/logs',
};

const ReportsNavGrouping = {
  rolesWithAccess: ['MPReportReader'],
  title: 'Reports',
  route: '/reports',
  subRoutes: [OrchestrationLogs],
};

const SellerUsers = {
  rolesWithAccess: ['MPSellerAdmin'],
  title: 'Seller Users',
  route: '/seller-users',
};

const Storefronts = {
  rolesWithAccess: ['MPStoreFrontAdmin'],
  title: 'StoreFronts',
  route: '/storefronts',
};

const PublicProfile = {
  rolesWithAccess: ['MPMeSupplierAdmin'],
  title: 'Public Profile',
  route: '/my-supplier-profile',
};

const AllNavGroupings: MPRoute[] = [
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

export const getHeaderConfig = (userRoles: string[], orderCloudUserType: string): MPRoute[] => {
  const navGroupingsApplicableToUser = filterOutNavGroupings(AllNavGroupings, userRoles, orderCloudUserType);
  return navGroupingsApplicableToUser.map(navGrouping => {
    if (!navGrouping.subRoutes) {
      return navGrouping;
    } else {
      const routesApplicableToUser = filterOutNavGroupings(navGrouping.subRoutes, userRoles, orderCloudUserType);
      navGrouping.subRoutes = routesApplicableToUser;
      return navGrouping;
    }
  });
};

const filterOutNavGroupings = (navGroupings: MPRoute[], userRoles: string[], orderCloudUserType: string): MPRoute[] => {
  return navGroupings.filter(navGrouping => {
    return (
      navGrouping.rolesWithAccess.some(role => userRoles.includes(role)) &&
      (!navGrouping.orderCloudUserTypesWithAccess ||
        navGrouping.orderCloudUserTypesWithAccess.includes(orderCloudUserType))
    );
  });
};
