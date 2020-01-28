import { HeaderNav } from './header.component';
import { FilterDictionary } from '@app-seller/shared/services/resource-crud/resource-crud.types';
import { SELLER, SUPPLIER } from '@app-seller/shared/models/ordercloud-user.types';

// ! included to ensure no overlap with ordercloud ids as this in invalid in ids
export const REDIRECT_TO_FIRST_PARENT = '!';

export interface MPRoute {
  rolesWithAccess: string[];
  // this allows the routes to be narrowed based upon OC user type
  orderCloudUserTypesWithAccess?: string[];
  title: string;
  route: string;
  queryParams?: FilterDictionary;

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
};

const PendingProducts: MPRoute = {
  rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'Pending Products',
  route: '/products',
};

const Promotions: MPRoute = {
  rolesWithAccess: ['MPPromotionAdmin', 'MPPromotionReader'],
  title: 'Promotions',
  route: '/promotions',
};

const Categories: MPRoute = {
  rolesWithAccess: ['MPCategoryAdmin', 'MPCategoryReader'],
  title: 'Categories',
  route: '/categories',
};

const ProductNavGrouping: MPRoute = {
  rolesWithAccess: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'Products',
  route: '/products',
  subRoutes: [AllProducts, LiveProducts, PendingProducts, Promotions, Categories],
};

//Orders
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

//Buyers
const AllBuyers: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'All Buyers',
  route: `/buyers`,
};

const BuyerUsers: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Users',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const BuyerPurchasingLocations: MPRoute = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Purchasing Locations',
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

const BuyerNavGrouping = {
  rolesWithAccess: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Buyers',
  route: '/buyers',
  subRoutes: [AllBuyers, BuyerUsers, BuyerPurchasingLocations, BuyerPaymentMethods, BuyerApprovalRules],
};

//Suppliers
const AllSuppliers: MPRoute = {
  rolesWithAccess: ['MPSupplierAdmin'],
  title: 'All Suppliers',
  route: `/suppliers`,
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

const ReportsRoute = {
  rolesWithAccess: ['MPReportReader'],
  title: 'Reports',
  route: '/reports',
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
  SellerUsers,
  ReportsRoute,
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
