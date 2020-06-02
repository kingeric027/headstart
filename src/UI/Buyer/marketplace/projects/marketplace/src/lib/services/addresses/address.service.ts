import { Injectable } from '@angular/core';
import { OcMeService, OcAddressService, OcTokenService } from '@ordercloud/angular-sdk';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';
import { TaxCertificate, MarketplaceAddressBuyer, MarketplaceSDK } from 'marketplace-javascript-sdk';
import { ListMarketplaceAddressBuyer, AppConfig } from '../../shopper-context';
import { CurrentUserService } from '../current-user/current-user.service';
import { UserManagementService } from '../user-management/user-management.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AddressService {
  avalaraCompanyId: number;
  constructor(
    private ocMeService: OcMeService,
    private appConfig: AppConfig,
    private ocAddressService: OcAddressService,
    private user: CurrentUserService,
    private userManagementService: UserManagementService,

    // remove below when sdk is regenerated
    private ocTokenService: OcTokenService,
    private httpClient: HttpClient
  ) {
    this.avalaraCompanyId = this.appConfig.avalaraCompanyId;
  }

  async get(addressID: string): Promise<MarketplaceAddressBuyer> {
    return this.ocMeService.GetAddress(addressID).toPromise();
  }

  async list(args: ListArgs): Promise<ListMarketplaceAddressBuyer> {
    return this.ocMeService.ListAddresses(args).toPromise();
  }

  async create(address: MarketplaceAddressBuyer): Promise<MarketplaceAddressBuyer> {
    return MarketplaceSDK.ValidatedAddresses.CreateMeAddress(address);
  }

  async edit(addressID: string, address: MarketplaceAddressBuyer): Promise<MarketplaceAddressBuyer> {
    return MarketplaceSDK.ValidatedAddresses.SaveMeAddress(addressID, address);
  }

  async delete(addressID: string): Promise<void> {
    return this.ocMeService.DeleteAddress(addressID).toPromise();
  }

  async listBuyerLocations(args: ListArgs = {}): Promise<ListMarketplaceAddressBuyer> {
    args.filters = { ...args.filters, Editable: 'false' };
    return await this.list(args);
  }

  async createCertificate(locationID: string, certificate: TaxCertificate): Promise<TaxCertificate> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/avalara/${this.avalaraCompanyId}/certificate/${locationID}`;
    return this.httpClient
      .post<TaxCertificate>(url, certificate, { headers })
      .toPromise();
  }

  async updateCertificate(locationID: number, certificate: TaxCertificate): Promise<TaxCertificate> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/avalara/${this.avalaraCompanyId}/certificate/${locationID}`;
    return this.httpClient
      .put<TaxCertificate>(url, certificate, { headers })
      .toPromise();
  }

  getCertificate = (locationID: string): Promise<TaxCertificate> => {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/avalara/${this.avalaraCompanyId}/certificate/${locationID}`;
    return this.httpClient
      .get<TaxCertificate>(url, { headers })
      .toPromise();
  };
}
