import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { OcMeService, ListBuyerAddress } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';

@Injectable()
export class MeListBuyerAddressResolver implements Resolve<ListBuyerAddress> {
  constructor(private service: OcMeService) {}

  resolve(): Observable<ListBuyerAddress> | Promise<ListBuyerAddress> | any {
    return this.service.ListAddresses();
  }
}
