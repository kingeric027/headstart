import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { OcMeService } from '@ordercloud/angular-sdk';
import { ListPage, MarketplaceSDK } from 'marketplace-javascript-sdk';
import { ExchangeRates } from '../../shopper-context';

@Injectable({
  providedIn: 'root',
})
export class ExchangeRatesService {
  private ratesSubject: BehaviorSubject<ListPage<ExchangeRates>> = new BehaviorSubject<ListPage<ExchangeRates>>(null);

  constructor(private ocMeService: OcMeService) {}

  Get(): ListPage<ExchangeRates> {
    return this.exchangeRates;
  }

  async reset(): Promise<void> {
    const myUserGroups = await this.ocMeService
      .ListUserGroups({ pageSize: 1, filters: { 'xp.Type': 'BuyerLocation' } })
      .toPromise();
    const baseRate = myUserGroups.Items[0].xp?.Currency;
    this.exchangeRates = await MarketplaceSDK.ExchangeRates.Get(baseRate);
  }

  private get exchangeRates(): ListPage<ExchangeRates> {
    return this.ratesSubject.value;
  }

  private set exchangeRates(value: ListPage<ExchangeRates>) {
    this.ratesSubject.next(value);
  }
}
