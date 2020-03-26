import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { OcMeService, ListBuyerAddress } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { CurrentUserAddressService } from '../services/current-user/address.service';

@Injectable()
export class MeListAddressResolver implements Resolve<ListBuyerAddress> {
  constructor(private service: OcMeService) {}

  resolve(): Observable<ListBuyerAddress> | Promise<ListBuyerAddress> | any {
    return this.service.ListAddresses();
  }
}

@Injectable()
export class MeListBuyerLocationResolver implements Resolve<ListBuyerAddress> {
  constructor(private service: CurrentUserAddressService) {}

  resolve(): Observable<ListBuyerAddress> | Promise<ListBuyerAddress> | any {
    return this.service.listBuyerLocationsWithCerts();
  }
}
