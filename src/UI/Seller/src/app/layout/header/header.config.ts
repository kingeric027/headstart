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
  title: 'ADMIN.NAV.ALL_PRODUCTS',
  route: '/products',
};

const LiveProducts: MPRoute = {
  rolesWithAccess: [MPRoles.MPProductAdmin, MPRoles.MPProductReader, MPRoles.MPMeProductAdmin],
  title: 'ADMIN.NAV.LIVE_PRODUCTS',
  route: '/products',
  queryParams: { 'xp.Status': 'Published' },
};

const PendingProducts: MPRoute = {
  rolesWithAccess: [MPRoles.MPProductAdmin, MPRoles.MPProductReader, MPRoles.MPMeProductAdmin],
  title: 'ADMIN.NAV.PENDING_PRODUCTS',
  route: '/products',
  queryParams: { 'xp.Status': 'Draft' },
};

const Promotions: MPRoute = {
  rolesWithAccess: [MPRoles.MPPromotionAdmin, MPRoles.MPPromotionReader],
  title: 'ADMIN.NAV.PROMOTIONS',
  route: '/promotions',
};

const Kits: MPRoute = {
  rolesWithAccess: [MPRoles.MPProductAdmin, MPRoles.MPProductReader, MPRoles.MPMeProductAdmin],
  title: 'Kits',
  route: '/kitproducts',
};

const ProductFacets: MPRoute = {
  rolesWithAccess: [MPRoles.MPStorefrontAdmin],
  title: 'ADMIN.NAV.FACETS',
  route: '/facets',
};

const ProductNavGrouping: MPRoute = {
  rolesWithAccess: [MPRoles.MPProductAdmin, MPRoles.MPProductReader, MPRoles.MPMeProductAdmin],
  title: 'ADMIN.NAV.PRODUCTS',
  route: '/products',
  subRoutes: [AllProducts, LiveProducts, PendingProducts, Promotions, ProductFacets, Kits],
};

// Orders
const BuyerOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.SALES_ORDERS',
  route: '/orders',
  queryParams: { OrderDirection: 'Incoming' },
};

const SupplierPurchaseOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.PURCHASE_ORDERS',
  route: '/orders',
  queryParams: { OrderDirection: 'Outgoing' },
};

const RequiringAttentionOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.NEEDING_ATTENTION',
  route: '/orders',
  queryParams: { OrderDirection: 'Incoming', 'xp.NeedsAttention': 'true' },
};

const Orders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.ORDERS',
  route: '/orders',
};

const AwaitingApprovalOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.AWAITING_APPROVAL_ORDERS',
  route: '/orders',
};

const ShippedOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.SHIPPED_ORDERS',
  route: '/orders',
};

const CancelledOrders: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.CANCELLED_ORDERS',
  route: '/orders',
};

const SellerOrderNavGrouping: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.ORDERS',
  route: '/orders',
  orderCloudUserTypesWithAccess: [SELLER],
  subRoutes: [BuyerOrders, SupplierPurchaseOrders, RequiringAttentionOrders],
};

const SupplierOrderNavGrouping: MPRoute = {
  rolesWithAccess: [MPRoles.MPOrderAdmin, MPRoles.MPOrderReader, MPRoles.MPShipmentAdmin],
  title: 'ADMIN.NAV.ORDERS',
  route: '/orders',
  orderCloudUserTypesWithAccess: [SUPPLIER],
  subRoutes: [Orders, AwaitingApprovalOrders, ShippedOrders, CancelledOrders],
};

// Buyers
const AllBuyers: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'ALIAS.ALL_BUYERS',
  route: '/buyers',
};

const BuyerUsers: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'ADMIN.NAV.USERS',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const BuyerPurchasingLocations: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'ALIAS.BUYER_LOCATIONS',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/locations`,
};

const BuyerPaymentMethods: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'ADMIN.NAV.PAYMENT_METHODS',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/payments`,
};

const BuyerApprovalRules: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'ADMIN.NAV.APPROVAL_RULES',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/approvals`,
};

const BuyerCatalogs: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'ADMIN.NAV.CATALOGS',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/catalogs`,
};

const BuyerCategories: MPRoute = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'ADMIN.NAV.CATEGORIES',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/categories`,
};

const BuyerNavGrouping = {
  rolesWithAccess: [MPRoles.MPBuyerAdmin, MPRoles.MPBuyerReader],
  title: 'ALIAS.BUYERS',
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
  title: 'ALIAS.ALL_SUPPLIERS',
  route: '/suppliers',
};

const SupplierUsers: MPRoute = {
  rolesWithAccess: [MPRoles.MPSupplierAdmin],
  title: 'ADMIN.NAV.USERS',
  route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const SupplierLocations: MPRoute = {
  rolesWithAccess: [MPRoles.MPSupplierAdmin, MPRoles.MPMeSupplierAdmin],
  title: 'ALIAS.SUPPLIER_LOCATIONS',
  route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/locations`,
};

const SupplierNavGrouping: MPRoute = {
  rolesWithAccess: [MPRoles.MPSupplierAdmin],
  title: 'ALIAS.SUPPLIERS',
  route: '/suppliers',
  subRoutes: [AllSuppliers, SupplierUsers, SupplierLocations],
};

const OrchestrationLogs = {
  rolesWithAccess: [MPRoles.MPReportReader],
  title: 'ADMIN.NAV.ORCHESTRATION_LOGS',
  route: 'reports/logs',
};

const ReportsNavGrouping = {
  rolesWithAccess: [MPRoles.MPReportReader],
  title: 'ADMIN.NAV.REPORTS',
  route: '/reports',
  subRoutes: [OrchestrationLogs],
};

const SellerUsers = {
  rolesWithAccess: [MPRoles.MPSellerAdmin],
  title: 'ADMIN.NAV.SELLER_USERS',
  route: '/seller-users',
};

const Storefronts = {
  rolesWithAccess: [MPRoles.MPStorefrontAdmin],
  title: 'ADMIN.NAV.STOREFRONTS',
  route: '/storefronts',
};

const MySupplierProfile = {
  rolesWithAccess: [MPRoles.MPMeSupplierAdmin],
  title: 'ALIAS.SUPPLIER_PROFILE',
  route: '/my-supplier',
};

const MySupplierLocations = {
  rolesWithAccess: [MPRoles.MPMeSupplierAddressAdmin],
  title: 'ALIAS.SUPPLIER_LOCATIONS',
  route: '/my-supplier/locations',
};

const MySupplerUsers = {
  rolesWithAccess: [MPRoles.MPMeSupplierUserAdmin],
  title: 'ADMIN.NAV.USERS',
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
