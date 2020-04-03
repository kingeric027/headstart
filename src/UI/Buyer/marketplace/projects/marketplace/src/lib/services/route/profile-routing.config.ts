export interface ProfileSection {
  routerCall: string;
  displayText: string;
  url: string;
  showInDropdown: boolean;

  // no roles with access means all users will see
  rolesWithAccess?: string[];
}

export const ProfileSections: ProfileSection[] = [
  {
    routerCall: 'toMyProfile',
    displayText: 'My Profile',
    url: '/profile/details',
    showInDropdown: true,
  },
  {
    routerCall: 'toMyAddresses',
    displayText: 'My Addresses',
    url: '/profile/addresses',
    showInDropdown: true,
  },
  {
    routerCall: 'toMyLocations',
    displayText: 'My Locations',
    url: '/profile/locations',
    showInDropdown: true,
  },
  {
    routerCall: 'toUsers',
    displayText: 'User Approval Management',
    url: '/profile/users',
    rolesWithAccess: ['MPApprovalRuleAdmin'],
    showInDropdown: true,
  },
  {
    routerCall: 'toMyPaymentMethods',
    displayText: 'My Credit Cards',
    url: '/profile/payment-methods',
    showInDropdown: true,
  },
  {
    routerCall: 'toMyOrders',
    displayText: 'Orders Submitted',
    url: '/profile/orders',
    showInDropdown: true,
  },
  {
    routerCall: 'toOrdersToApprove',
    displayText: 'Orders To Approve',
    url: '/profile/orders/approve',
    rolesWithAccess: ['MPOrderApprover'],
    showInDropdown: true,
  },
  {
    routerCall: 'toOrderApprovalFlow',
    displayText: 'Order Approval Flow',
    url: '/profile/orders/getapproval',
    rolesWithAccess: ['MPNeedsApproval'],
    showInDropdown: true,
  },
  {
    routerCall: 'toChangePassword',
    displayText: 'Change Password',
    url: '/profile/details',
    showInDropdown: false,
  },
];
