import * as jwtDecode from 'jwt-decode';
import { DecodedOrderCloudToken } from '@app-seller/shared';
export const getRolesFromToken = (token: string): string[] => {
  let decodedToken: DecodedOrderCloudToken;
  try {
    decodedToken = jwtDecode(token);
  } catch (e) {
    decodedToken = null;
  }
  if (!decodedToken) {
    throw new Error('decoded jwt was null when attempting to get user type');
  }
  return decodedToken.role;
};
