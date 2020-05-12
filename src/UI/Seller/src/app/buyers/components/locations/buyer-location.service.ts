import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcAddressService, BuyerAddress } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from '../buyers/buyer.service';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';

// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerLocationService extends ResourceCrudService<BuyerAddress> {
  emptyResource = {
    UserGroup: {
      xp: {
        Type: '',
        Currency: null,
      },
      ID: '',
      Name: '',
      Description: '',
    },
    Address: {
      xp: {
        Email: '',
      },
      ID: '',
      DateCreated: '',
      CompanyName: '',
      FirstName: '',
      LastName: '',
      Street1: '',
      Street2: '',
      City: '',
      State: '',
      Zip: '',
      Country: '',
      Phone: '',
      AddressName: '',
    },
  };

  constructor(router: Router, activatedRoute: ActivatedRoute, ocAddressService: OcAddressService) {
    super(router, activatedRoute, ocAddressService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'locations');
  }

  async updateResource(originalID: string, resource: any): Promise<any> {
    const newResource = await MarketplaceSDK.ValidatedAddresses.SaveBuyerAddress(
      this.getParentResourceID(),
      originalID,
      resource
    );
    const resourceIndex = this.resourceSubject.value.Items.findIndex((i: any) => i.ID === newResource.ID);
    this.resourceSubject.value.Items[resourceIndex] = newResource;
    this.resourceSubject.next(this.resourceSubject.value);
    return newResource;
  }

  async createNewResource(resource: any): Promise<any> {
    const newResource = await MarketplaceSDK.ValidatedAddresses.CreateBuyerAddress(
      this.getParentResourceID(),
      resource
    );
    this.resourceSubject.value.Items = [...this.resourceSubject.value.Items, newResource];
    this.resourceSubject.next(this.resourceSubject.value);
    return newResource;
  }
}
