import { Injectable, Inject } from '@angular/core';
import { OcUserService, ApiRole } from '@ordercloud/angular-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';

@Injectable({
  providedIn: 'root',
})
export class ImpersonationService {
  constructor(private userService: OcUserService, @Inject(applicationConfiguration) private appConfig: AppConfig) {}

  async impersonateUser(networkID: string, user: any): Promise<void> {
    const auth = await this.userService
      .GetAccessToken(networkID, user.ID, {
        ClientID: this.appConfig.buyerClientID,
        Roles: this.appConfig.impersonatingBuyerScope,
      })
      .toPromise();
    const url = this.appConfig.buyerUrl + 'impersonation?token=' + auth.access_token;
    window.open(url, '_blank');
  }
}