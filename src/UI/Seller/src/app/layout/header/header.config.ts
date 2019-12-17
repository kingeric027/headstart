import { HeaderNav } from './header.component';

// ! included to ensure no overlap with ordercloud ids as this in invalid in ids
export const REDIRECT_TO_FIRST_PARENT = '!';

export interface MPRoute {
  activatorRoles: string[];
  title: string;
  route: string;

  // if subroutes are included, itesms will display in a dropdown
  subRoutes?: MPRoute[];
}

// Products
const LiveProducts: MPRoute = {
  activatorRoles: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'Live Products',
  route: '/products',
};

const PendingProducts: MPRoute = {
  activatorRoles: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'Pending Products',
  route: '/products',
};

const Promotions: MPRoute = {
  activatorRoles: ['MPPromotionAdmin', 'MPPromotionReader'],
  title: 'Promotions',
  route: '/promotions',
};

const Categories: MPRoute = {
  activatorRoles: ['MPCategoryAdmin', 'MPCategoryReader'],
  title: 'Categories',
  route: '/categories',
};

const ProductNavGrouping: MPRoute = {
  activatorRoles: ['MPPRoductAdmin', 'MPProductReader', 'MPMeProductAdmin'],
  title: 'Products',
  route: '/products',
  subRoutes: [LiveProducts, PendingProducts, Promotions, Categories],
};

//Orders
const OpenOrders: MPRoute = {
  activatorRoles: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Open Orders',
  route: '/orders',
};

const AwaitingApprovalOrders: MPRoute = {
  activatorRoles: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Awaiting Approval Orders',
  route: '/orders',
};

const ShippedOrders: MPRoute = {
  activatorRoles: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Shipped Orders',
  route: '/orders',
};

const CancelledOrders: MPRoute = {
  activatorRoles: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Cancelled Orders',
  route: '/orders',
};

const OrderNavGrouping: MPRoute = {
  activatorRoles: ['MPOrderAdmin', 'MPOrderReader', 'MPShipmentAdming'],
  title: 'Orders',
  route: '/orders',
  subRoutes: [OpenOrders, AwaitingApprovalOrders, ShippedOrders, CancelledOrders],
};

//Buyers
const BuyerUsers: MPRoute = {
  activatorRoles: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Users',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const BuyerPurchasingLocations: MPRoute = {
  activatorRoles: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Purchasing Locations',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/locations`,
};

const BuyerPaymentMethods: MPRoute = {
  activatorRoles: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Payment Methods',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/payments`,
};

const BuyerApprovalRules: MPRoute = {
  activatorRoles: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Approval Rules',
  route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/approvals`,
};

const BuyerNavGrouping = {
  activatorRoles: ['MPBuyerAdmin', 'MPBuyerReader'],
  title: 'Buyers',
  route: '/buyers',
  subRoutes: [BuyerUsers, BuyerPurchasingLocations, BuyerPaymentMethods, BuyerApprovalRules],
};

//Suppliers
const SupplierUsers: MPRoute = {
  activatorRoles: ['MPSupplierAdmin'],
  title: 'Users',
  route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/users`,
};

const SupplierLocations: MPRoute = {
  activatorRoles: ['MPSupplierAdmin', 'MPMeSu'],
  title: 'Locations',
  route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/locations`,
};

const SupplierNavGrouping: MPRoute = {
  activatorRoles: ['MPSupplierAdmin'],
  title: 'Suppliers',
  route: '/suppliers',
  subRoutes: [SupplierUsers, SupplierLocations],
};

const ReportsRoute = {
  activatorRoles: ['MPReportReader'],
  title: 'Reports',
  route: '/reports',
};

const SellerUsers = {
  activatorRoles: ['MPSellerAdmin'],
  title: 'Seller Users',
  route: '/seller-users',
};

const Storefronts = {
  activatorRoles: ['MPStoreFrontAdmin'],
  title: 'StoreFronts',
  route: '/storefronts',
};

const PublicProfile = {
  activatorRoles: ['MPMeSupplierAdmin'],
  title: 'Public Profile',
  route: '/my-supplier-profile',
};

const AllNavGroupings: MPRoute[] = [
  ProductNavGrouping,
  OrderNavGrouping,
  BuyerNavGrouping,
  SupplierNavGrouping,
  SellerUsers,
  ReportsRoute,
  Storefronts,
  PublicProfile,
];

export const getHeaderConfig = (userRoles: string[]): MPRoute[] => {
  return AllNavGroupings;

  // returning all nav groupings for now until more users are created that have the proper roles
  // const navGroupingsApplicableToUser = AllNavGroupings.filter((navGrouping) => {
  //   return navGrouping.activatorRoles.some((role) => userRoles.includes(role));
  // });
  // return navGroupingsApplicableToUser.map((navGrouping) => {
  //   if (!navGrouping.subRoutes) {
  //     return navGrouping;
  //   } else {
  //     const routesApplicableToUser = navGrouping.subRoutes.filter((subRoute) => {
  //       return subRoute.activatorRoles.some((role) => userRoles.includes(role));
  //     });
  //     navGrouping.subRoutes = routesApplicableToUser;
  //     return navGrouping;
  //   }
  // });
  // return navGroupingsApplicableToUser;
};
