import { Injectable } from '@angular/core';
import { OcMeService, BuyerAddress, ListBuyerAddress } from '@ordercloud/angular-sdk';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';
import BuyerLocations from 'marketplace-javascript-sdk/dist/api/BuyerLocations';
import { TaxCertificate, MarketplaceAddressBuyer, BuyerAddressXP, MarketplaceSDK } from 'marketplace-javascript-sdk';
import { ListMarketplaceAddressBuyer, AppConfig, BuyerLocationWithCert } from '../../shopper-context';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserAddressService {
  companyId: number;
  constructor(private ocMeService: OcMeService, private appConfig: AppConfig) {
    this.companyId = this.appConfig.avalaraCompanyId;
  }

  async get(addressID: string): Promise<MarketplaceAddressBuyer> {
    return this.ocMeService.GetAddress(addressID).toPromise();
  }

  async list(args: ListArgs): Promise<ListMarketplaceAddressBuyer> {
    return this.ocMeService.ListAddresses(args).toPromise();
  }

  async listShipping(args: ListArgs = {}): Promise<ListMarketplaceAddressBuyer> {
    args.filters = { ...args.filters, Shipping: 'true' };
    return this.list(args);
  }

  async listBilling(args: ListArgs = {}): Promise<ListMarketplaceAddressBuyer> {
    args.filters = { ...args.filters, Billing: 'true' };
    return this.list(args);
  }

  async create(address: MarketplaceAddressBuyer): Promise<MarketplaceAddressBuyer> {
    return this.ocMeService.CreateAddress(address).toPromise();
  }

  async edit(addressID: string, address: MarketplaceAddressBuyer): Promise<MarketplaceAddressBuyer> {
    return this.ocMeService.SaveAddress(addressID, address).toPromise();
  }

  async delete(addressID: string): Promise<void> {
    return this.ocMeService.DeleteAddress(addressID).toPromise();
  }

  async listBuyerLocations(args: ListArgs = {}): Promise<ListMarketplaceAddressBuyer> {
    args.filters = { ...args.filters, Editable: 'false' };
    return await this.list(args);
  }

  async listBuyerLocationsWithCerts(args: ListArgs = {}): Promise<BuyerLocationWithCert[]> {
    const addresses = await this.listBuyerLocations(args);
    const certificates = await Promise.all(addresses.Items.map(this.getCertificate));
    return addresses.Items.map((address, i) => ({ location: address, certificate: certificates[i] }));
  }

  async createCertificate(locationID: string, certificate: TaxCertificate): Promise<TaxCertificate> {
    const created = await MarketplaceSDK.Avalaras.CreateCertificate(this.companyId, certificate);
    await this.ocMeService.PatchAddress(locationID, { xp: { AvalaraCertificateID: created.ID } }).toPromise();
    return created;
  }

  async updateCertificate(certificateID: number, certificate: TaxCertificate): Promise<TaxCertificate> {
    const created = await MarketplaceSDK.Avalaras.UpdateCertificate(this.companyId, certificateID, certificate);
    return created;
  }

  private getCertificate = (address: MarketplaceAddressBuyer): Promise<TaxCertificate> => {
    const certificateID = address?.xp?.AvalaraCertificateID;
    if (certificateID === null || certificateID === undefined) return Promise.resolve(null);
    return MarketplaceSDK.Avalaras.GetCertificate(this.companyId, certificateID);
  };
}
