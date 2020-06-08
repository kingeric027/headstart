import { ExchangeRates } from 'marketplace/projects/marketplace/src/lib/services/exchange-rates/exchange-rates.service';
import { ListPage } from '../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';
import { BuyerCurrency } from '../models/currency.interface';

export function exchange(
  rates: ListPage<ExchangeRates>,
  price: number,
  productCurrency: string,
  orderCurrency: string
): BuyerCurrency {
  const targetRate = rates.Items.find(r => r.Currency === productCurrency);
  return {
    Price: price / targetRate?.Rate,
    Currency: orderCurrency,
  };
}
