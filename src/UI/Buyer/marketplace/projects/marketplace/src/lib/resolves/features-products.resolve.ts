import { Resolve } from '@angular/router';
import { OcMeService } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ListMarketplaceMeProduct, MarketplaceMeProduct } from '../shopper-context';

@Injectable()
export class FeaturedProductsResolver implements Resolve<MarketplaceMeProduct> {
  constructor(private service: OcMeService) {}

  resolve(): Observable<ListMarketplaceMeProduct> | Promise<ListMarketplaceMeProduct> | any {
    return this.service.ListProducts({ filters: { 'xp.Featured': true } as any });
  }
}
