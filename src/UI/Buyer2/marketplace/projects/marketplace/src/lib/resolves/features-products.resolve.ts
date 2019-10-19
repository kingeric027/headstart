import { Resolve } from '@angular/router';
import { BuyerProduct, OcMeService, ListBuyerProduct } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class FeaturedProductsResolver implements Resolve<BuyerProduct> {
  constructor(private service: OcMeService) {}

  resolve(): Observable<ListBuyerProduct> | Promise<ListBuyerProduct> | any {
    return this.service.ListProducts({ filters: { 'xp.Featured': true } as any });
  }
}
