import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { OcMeService } from '@ordercloud/angular-sdk';
import { HttpClient } from '@angular/common/http';
import { ListPage, MarketplaceSDK } from 'marketplace-javascript-sdk';
import { AppConfig } from '../../shopper-context';

export interface ExchangeRates {
  Currency: string;
  Symbol: string;
  Name: string;
  Rate: number;
  Icon: string;
}

export interface IExchangeRates {
  Get(): ListPage<ExchangeRates>;
}

@Injectable({
  providedIn: 'root',
})
export class ExchangeRatesService implements IExchangeRates {
  private ratesSubject: BehaviorSubject<ListPage<ExchangeRates>> = new BehaviorSubject<ListPage<ExchangeRates>>(null);

  constructor(private ocMeService: OcMeService, public http: HttpClient, private appConfig: AppConfig) {}

  Get(): ListPage<ExchangeRates> {
    return this.exchangeRates;
  }

  async reset(): Promise<void> {
    const myUserGroups = await this.ocMeService.ListUserGroups({ pageSize: 1 }).toPromise();
    const baseRate = myUserGroups.Items[0].xp?.Currency || 'USD';
    this.exchangeRates = await MarketplaceSDK.ExchangeRates.Get(baseRate);
  }

  private get exchangeRates(): ListPage<ExchangeRates> {
    return this.ratesSubject.value;
  }

  private set exchangeRates(value: ListPage<ExchangeRates>) {
    this.ratesSubject.next(value);
  }
}
