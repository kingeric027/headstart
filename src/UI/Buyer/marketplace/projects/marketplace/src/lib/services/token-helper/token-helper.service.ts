import { Injectable } from '@angular/core';
import * as jwtDecode_ from 'jwt-decode';
const jwtDecode = jwtDecode_;
import { isUndefined as _isUndefined } from 'lodash';
import { DecodedOCToken } from '../../shopper-context';
import { Tokens } from 'ordercloud-javascript-sdk';

@Injectable({
  providedIn: 'root',
})
export class TokenHelperService {
  constructor() {}

  getDecodedOCToken(): DecodedOCToken {
    try {
      return jwtDecode(Tokens.GetAccessToken());
    } catch (e) {
      return null;
    }
  }

  isTokenAnonymous(): boolean {
    return !_isUndefined(this.getAnonymousOrderID());
  }

  getAnonymousOrderID(): string | null {
    const token = this.getDecodedOCToken();
    return token ? token.orderid : null;
  }
}
