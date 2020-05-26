import { ExchangeRates } from 'marketplace/projects/marketplace/src/lib/services/exchange-rates/exchange-rates.service';
import { BuyerCurrency } from '../components/products/product-card/product-card.component';
import { ListPage } from '../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';

export function exchange(
  rates: ListPage<ExchangeRates>,
  price: number,
  productCurrency: string,
  orderCurrency: string
): BuyerCurrency {
  const targetRate = rates.Items.find(r => r.Currency === (productCurrency || 'USD'));
  return {
    Price: price / targetRate.Rate,
    Currency: orderCurrency,
  };
}
