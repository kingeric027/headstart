import { SELLER, SUPPLIER } from '@app-seller/shared/models/ordercloud-user.types';
import { MPRoles } from '@app-seller/config/mp-security-profiles';

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
  rolesWithAccess: [MPRoles.MPProductAdmin, MPRoles.MPProductReader, MPRoles.MPMeProductAdmin],
  title: 'NAV.PRODUCTS.ALL_PRODUCTS',
  route: '/products',
};

const LiveProducts: MPRoute = {
  rolesWithAccess: [MPRoles.MPProductAdmin, MPRoles.MPProductReader, MPRoles.MPMeProductAdmin],
  title: 'NAV.PRODUCTS.LIVE_PRODUCTS',
  route: '/products',
  queryParams: { 'xp.Status': 'Published' },
};

const PendingProducts: MPRoute = {
  rolesWithAccess: [MPRoles.MPProductAdmin, MPRoles.MPProductReader, MPRoles.MPMeProductAdmin],
  title: 'NAV.PRODUCTS.PENDING_PRODUCTS',
  route: '/products',
  queryParams: { 'xp.Status': 'Draft' },
};

const Promotions: MPRoute = {
  rolesWithAccess: [MPRoles.MPPromotionAdmin, MPRoles.MPPromotionReader],
  title: 'NAV.PRODUCTS.PROMOTIONS',
  route: '/promotions',
};

const ProductFacets: MPRoute = {
  rolesWithAccess: [MPRoles.MPStoreFrontAdmin],
  title: 'NAV.PRODUCTS.FACETS',
  route: '/facets',
};

const ProductNavGrouping: MPRoute = {
  rolesWithAccess: [MPRoles.MPProductAdmin, MPRoles.MPProductReader, MPRoles.MPMeProductAdmin],
  title: 'NAV.PRODUCTS.PRODUCTS',
  route: '/products',
  subRoutes: [AllProducts, LiveProducts, PendingProducts, Promotions, ProductFacets],
};

// Orders
const BuyerOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'Sales Orders',
  route: '/orders',
  queryParams: { OrderDirection: 'Incoming' },
};

const SupplierPurchaseOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'Purchase Orders',
  route: '/orders',
  queryParams: { OrderDirection: 'Outgoing' },
};

const RequiringAttentionOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'Needing Attention',
  route: '/orders',
  queryParams: { OrderDirection: 'Incoming', 'xp.NeedsAttention': 'true' },
};

const Orders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'NAV.ORDERS.ORDERS',
  route: '/orders',
};

const AwaitingApprovalOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'Awaiting Approval Orders',
  route: '/orders',
};

const ShippedOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'Shipped Orders',
  route: '/orders',
};

const CancelledOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'Cancelled Orders',
  route: '/orders',
};

const SellerOrderNavGrouping: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'Orders',
  route: '/orders',
  orderCloudUserTypesWithAccess: [SELLER],
  subRoutes: [BuyerOrders, SupplierPurchaseOrders, RequiringAttentionOrders],
};

const SupplierOrderNavGrouping: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'Orders',
  route: '/orders',
  orderCloudUserTypesWithAccess: [SUPPLIER],
  subRoutes: [Orders, AwaitingApprovalOrders, ShippedOrders, CancelledOrders],
};

// Buyers
const AllBuyers: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'All Buyers',
  route: '/buyers',
};

const BuyerUsers: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'Users',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const BuyerPurchasingLocations: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'Locations',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/locations`,
};

const BuyerPaymentMethods: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'Payment Methods',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/payments`,
};

const BuyerApprovalRules: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'Approval Rules',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/approvals`,
};

const BuyerCatalogs: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'Catalogs',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/catalogs`,
};

const BuyerCategories: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'Categories',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/categories`,
};

const BuyerNavGrouping = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'Buyers',
  route: '/buyers',
  subRoutes: [
    AllBuyers,
    BuyerUsers,
    BuyerPurchasingLocations,
    BuyerPaymentMethods,
    BuyerApprovalRules,
    BuyerCatalogs,
    BuyerCategories,
  ],
};

// Suppliers
const AllSuppliers: MPRoute = {
  rolesWithAccess: [MPRoles.MPSupplierAdmin],
  title: 'All Suppliers',
  route: '/suppliers',
};

const SupplierUsers: MPRoute = {
  rolesWithAccess: [MPRoles.MPSupplierAdmin],
  title: 'Users',
  route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const SupplierLocations: MPRoute = {
  rolesWithAccess: [MPRoles.MPSupplierAdmin, MPRoles.MPMeSupplierAdmin],
  title: 'Locations',
  route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/locations`,
};

const SupplierNavGrouping: MPRoute = {
  rolesWithAccess: [MPRoles.MPSupplierAdmin],
  title: 'Suppliers',
  route: '/suppliers',
  subRoutes: [AllSuppliers, SupplierUsers, SupplierLocations],
};

const OrchestrationLogs = {
  rolesWithAccess: [MPRoles.MPReportReader],
  title: 'Orchestration Logs',
  route: 'reports/logs',
};

const ReportsNavGrouping = {
  rolesWithAccess: [MPRoles.MPReportReader],
  title: 'Reports',
  route: '/reports',
  subRoutes: [OrchestrationLogs],
};

const SellerUsers = {
  rolesWithAccess: [MPRoles.MPSellerAdmin],
  title: 'Seller Users',
  route: '/seller-users',
};

const Storefronts = {
  rolesWithAccess: [MPRoles.MPStoreFrontAdmin],
  title: 'StoreFronts',
  route: '/storefronts',
};

const MySupplierProfile = {
  rolesWithAccess: [MPRoles.MPMeSupplierAdmin],
  title: 'My Profile',
  route: '/my-supplier',
};

const MySupplierLocations = {
  rolesWithAccess: [MPRoles.MPMeSupplierAddressAdmin],
  title: 'Locations',
  route: '/my-supplier/locations',
};

const MySupplerUsers = {
  rolesWithAccess: [MPRoles.MPMeSupplierUserAdmin],
  title: 'Users',
  route: '/my-supplier/users',
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
  MySupplierProfile,
  MySupplierLocations,
  MySupplerUsers,
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
