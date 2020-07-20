import { Injectable } from '@angular/core';
import { Me } from 'ordercloud-javascript-sdk';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';
import { TaxCertificate, MarketplaceAddressBuyer, MarketplaceSDK, ListPage } from 'marketplace-javascript-sdk';
import { AppConfig } from '../../shopper-context';

@Injectable({
  providedIn: 'root',
})
export class AddressService {
  constructor(
    private appConfig: AppConfig // remove below when sdk is regenerated
  ) {}

  async get(addressID: string): Promise<MarketplaceAddressBuyer> {
    return Me.GetAddress(addressID);
  }

  async list(args: ListArgs): Promise<ListPage<MarketplaceAddressBuyer>> {
    return Me.ListAddresses(args as any);
  }

  async create(address: MarketplaceAddressBuyer): Promise<MarketplaceAddressBuyer> {
    return MarketplaceSDK.ValidatedAddresses.CreateMeAddress(address);
  }

  async edit(addressID: string, address: MarketplaceAddressBuyer): Promise<MarketplaceAddressBuyer> {
    return MarketplaceSDK.ValidatedAddresses.SaveMeAddress(addressID, address);
  }

  async delete(addressID: string): Promise<void> {
    return Me.DeleteAddress(addressID);
  }

  async listBuyerLocations(args: ListArgs = {}): Promise<ListPage<MarketplaceAddressBuyer>> {
    args.filters = { ...args.filters, Editable: 'false' };
    return await this.list(args);
  }

  async createCertificate(locationID: string, certificate: TaxCertificate): Promise<TaxCertificate> {
    return await MarketplaceSDK.Avalaras.CreateCertificate(this.appConfig.avalaraCompanyId, locationID, certificate);
  }

  async updateCertificate(locationID: string, certificate: TaxCertificate): Promise<TaxCertificate> {
    return await MarketplaceSDK.Avalaras.UpdateCertificate(this.appConfig.avalaraCompanyId, locationID, certificate);
  }

  async getCertificate(locationID: string): Promise<TaxCertificate> {
    return await MarketplaceSDK.Avalaras.GetCertificate(this.appConfig.avalaraCompanyId, locationID);
  }
}
