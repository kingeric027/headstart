import { Injectable } from '@angular/core';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { DecodedOCToken } from 'shopper-context-interface';
import * as jwtDecode from 'jwt-decode';
import { isUndefined as _isUndefined } from 'lodash';

@Injectable({
  providedIn: 'root',
})
export class TokenHelperService {
  constructor(private ocTokenService: OcTokenService) {}

  getDecodedOCToken(): DecodedOCToken {
    try {
      return jwtDecode(this.ocTokenService.GetAccess());
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
