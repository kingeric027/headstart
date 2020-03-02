import { Resolve } from '@angular/router';
import { OcMeService } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { MarketplaceProduct, ListMarketplaceProduct } from '../shopper-context';

@Injectable()
export class FeaturedProductsResolver implements Resolve<MarketplaceProduct> {
  constructor(private service: OcMeService) {}

  resolve(): Observable<ListMarketplaceProduct> | Promise<ListMarketplaceProduct> | any {
    return this.service.ListProducts({ filters: { 'xp.Featured': true } as any });
  }
}
