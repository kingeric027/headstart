import { MeUser } from '@ordercloud/angular-sdk';

export interface UserContext {
  Me: MeUser;
  UserRoles: string[];
  UserType: string;
}
