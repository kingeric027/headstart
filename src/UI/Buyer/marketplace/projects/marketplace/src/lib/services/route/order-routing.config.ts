import { RouteConfig } from './route-config';
import { OrderViewContext } from '../../shopper-context';

export const OrderRoutes: RouteConfig[] = [
  {
    routerCall: 'toMyOrders',
    displayText: 'Placed by Me',
    url: '/orders',
    showInDropdown: true,
    context: OrderViewContext.MyOrders,
  },
  {
    routerCall: 'toOrdersByLocation',
    displayText: 'Placed in My Locations',
    url: '/orders/location',
    rolesWithAccess: ['MPViewAllLocationOrders'],
    showInDropdown: true,
    context: OrderViewContext.Location,
  },
  {
    routerCall: 'toOrdersToApprove',
    displayText: 'Awaiting My Approval',
    url: '/orders/approve',
    rolesWithAccess: ['MPOrderApprover'],
    showInDropdown: true,
    context: OrderViewContext.Approve,
  },
];