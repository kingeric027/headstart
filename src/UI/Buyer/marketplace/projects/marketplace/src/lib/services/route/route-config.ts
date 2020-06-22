export interface RouteConfig {
  routerCall: string;
  displayText: string;
  url: string;
  showInDropdown: boolean;
  // no roles with access means all users will see
  rolesWithAccess?: string[];
  context?: string;
}
