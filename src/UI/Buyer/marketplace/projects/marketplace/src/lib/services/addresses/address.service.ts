import { Injectable } from '@angular/core';
import { OcMeService, OcAddressService } from '@ordercloud/angular-sdk';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';
import { TaxCertificate, MarketplaceAddressBuyer, MarketplaceSDK } from 'marketplace-javascript-sdk';
import { ListMarketplaceAddressBuyer, AppConfig, BuyerLocationWithCert } from '../../shopper-context';
import { CurrentUserService } from '../current-user/current-user.service';

@Injectable({
  providedIn: 'root',
})
export class AddressService {
  avalaraCompanyId: number;
  constructor(
    private ocMeService: OcMeService,
    private appConfig: AppConfig,
    private ocAddressService: OcAddressService,
    private user: CurrentUserService
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

  async listBuyerLocationsWithCerts(args: ListArgs = {}): Promise<BuyerLocationWithCert[]> {
    const addresses = await this.listBuyerLocations(args);
    const certificates = await Promise.all(addresses.Items.map(this.getCertificate));
    return addresses.Items.map((address, i) => ({ location: address, certificate: certificates[i] }));
  }

  async createCertificate(locationID: string, certificate: TaxCertificate): Promise<TaxCertificate> {
    const buyerID = this.user.get().Buyer.ID;
    const created = await MarketplaceSDK.Avalaras.CreateCertificate(this.avalaraCompanyId, certificate);
    await this.ocAddressService
      .Patch(buyerID, locationID, {
        xp: { AvalaraCertificateID: created.ID, AvalaraCertificateExpiration: created.ExpirationDate },
      })
      .toPromise();
    return created;
  }

  async updateCertificate(certificateID: number, certificate: TaxCertificate): Promise<TaxCertificate> {
    const created = await MarketplaceSDK.Avalaras.UpdateCertificate(this.avalaraCompanyId, certificateID, certificate);
    return created;
  }

  private getCertificate = (address: MarketplaceAddressBuyer): Promise<TaxCertificate> => {
    const certificateID = address?.xp?.AvalaraCertificateID;
    if (certificateID === null || certificateID === undefined) return Promise.resolve(null);
    return MarketplaceSDK.Avalaras.GetCertificate(this.avalaraCompanyId, certificateID);
  };
}
