import { HeaderNav } from './header.component';

// ! included to ensure no overlap with ordercloud ids as this in invalid in ids
export const REDIRECT_TO_FIRST_PARENT = 'automatically_selecting!';

export const MarketMangagerHeaderConfig: HeaderNav[] = [
  {
    title: 'Products',
    routes: [
      {
        title: 'All Products',
        route: '/products',
      },
      {
        title: 'Live Products',
        route: '/products',
      },
      {
        title: 'Pending Products',
        route: '/products',
      },
      {
        title: 'Promotions',
        route: '/promotions',
      },
      {
        title: 'Categories',
        route: '/categories',
      },
    ],
  },
  {
    title: 'Orders',
    routes: [
      {
        title: 'All Orders',
        route: '/orders',
      },
      {
        title: 'Open Orders',
        route: '/orders',
      },
      {
        title: 'Awaiting Approval',
        route: '/orders',
      },
      {
        title: 'Shipped Orders',
        route: '/orders',
      },
      {
        title: 'Canceled Orders',
        route: '/orders',
      },
    ],
  },
  {
    title: 'Buyers',
    routes: [
      {
        title: 'Organizations',
        route: '/buyers',
      },
      {
        title: 'Users',
        route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/users`,
      },
      {
        title: 'Purchasing Locations',
        route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/locations`,
      },
      {
        title: 'Shared Payment Methods',
        route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/payments`,
      },
      {
        title: 'Order Approval Rules',
        route: `/buyers/${REDIRECT_TO_FIRST_PARENT}/approvals`,
      },
    ],
  },
  {
    title: 'Suppliers',
    routes: [
      {
        title: 'Organizations',
        route: '/suppliers',
      },
      {
        title: 'Users',
        route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/users`,
      },
      {
        title: 'Locations',
        route: `/suppliers/${REDIRECT_TO_FIRST_PARENT}/locations`,
      },
    ],
  },
  {
    title: 'Seller Users',
    routes: [
      {
        title: 'Seller Users',
        route: '/seller-users',
      },
    ],
  },
  {
    title: 'Reports',
    routes: [
      {
        title: 'All Reports',
        route: '/reports',
      },
    ],
  },
  {
    title: 'Storefronts',
    routes: [
      {
        title: 'All Storefronts',
        route: '/storefronts',
      },
    ],
  },
];

export const SellerHeaderConfig: HeaderNav[] = [
  {
    title: 'Products',
    routes: [
      {
        title: 'All Products',
        route: '/products',
      },
      {
        title: 'Live Products',
        route: '/products',
      },
      {
        title: 'Pending Products',
        route: '/products',
      },
      {
        title: 'Promotions',
        route: '/promotions',
      },
      {
        title: 'Categories',
        route: '/categories',
      },
    ],
  },
  {
    title: 'Orders',
    routes: [
      {
        title: 'All Orders',
        route: '/orders',
      },
      {
        title: 'Open Orders',
        route: '/orders',
      },
      {
        title: 'Awaiting Approval',
        route: '/orders',
      },
      {
        title: 'Shipped Orders',
        route: '/orders',
      },
      {
        title: 'Canceled Orders',
        route: '/orders',
      },
    ],
  },
  {
    title: 'Buyers',
    routes: [
      {
        title: 'All Buyers',
        route: '/buyers',
      },
      {
        title: 'Create Buyer +',
        route: '/buyers/new',
      },
      {
        title: 'Users',
        route: '/buyers/abcd/users',
      },
      {
        title: 'Purchasing Locations',
        route: '/buyers/abcd/locations',
      },
      {
        title: 'Payment Methods',
        route: '/buyers/abcd/payments',
      },
      {
        title: 'Approval Rules',
        route: '/buyers/abcd/approvals',
      },
    ],
  },
  {
    title: 'Suppliers',
    routes: [
      {
        title: 'All Suppliers',
        route: '/suppliers',
      },
      {
        title: 'Add Supplier +',
        route: '/suppliers/new',
      },
      {
        title: 'Users',
        route: '/suppliers/abcd/users',
      },
      {
        title: 'Shipping Locations',
        route: '/suppliers/abcd/locations',
      },
    ],
  },
  {
    title: 'Reports',
    routes: [
      {
        title: 'All Reports',
        route: '/reports',
      },
    ],
  },
];

export const SupplierHeaderConfig: HeaderNav[] = [
  {
    title: 'Products',
    routes: [
      {
        title: 'All Products',
        route: '/products',
      },
      {
        title: 'Live Products',
        route: '/products',
      },
      {
        title: 'Pending Products',
        route: '/products',
      },
      {
        title: 'Promotions',
        route: '/promotions',
      },
      {
        title: 'Categories',
        route: '/categories',
      },
    ],
  },
  {
    title: 'Orders',
    routes: [
      {
        title: 'All Orders',
        route: '/orders',
      },
      {
        title: 'Open Orders',
        route: '/orders',
      },
      {
        title: 'Awaiting Approval',
        route: '/orders',
      },
      {
        title: 'Shipped Orders',
        route: '/orders',
      },
      {
        title: 'Canceled Orders',
        route: '/orders',
      },
    ],
  },
  {
    title: 'My Organization',
    routes: [
      {
        title: 'Public Profile',
        route: '/suppliers/abcd',
      },
      {
        title: 'Users',
        route: '/suppliers/abcd/users',
      },
      {
        title: 'Shipping Locations',
        route: '/suppliers/abcd/locations',
      },
    ],
  },
];
