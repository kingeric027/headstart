import { ExchangeRates } from 'marketplace/projects/marketplace/src/lib/services/exchange-rates/exchange-rates.service';
import { BuyerCurrency } from '../components/products/product-card/product-card.component';
import { ListPage } from '../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';

export function exchange(rates: ListPage<ExchangeRates>, price: number, productCurrency: string): BuyerCurrency {
  const targetRate = rates.Items.find(r => r.Currency === (productCurrency || 'USD'));
  const myRate = rates.Items.find(r => r.Rate === 1 || r.Rate === 0);
  return {
    Price: price / targetRate.Rate,
    Currency: myRate.Currency,
  };
}
