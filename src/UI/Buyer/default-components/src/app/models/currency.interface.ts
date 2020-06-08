export interface BuyerCurrency {
  Price: number;
  Currency: string;
}
export interface ExchangedPriceBreak {
  Price: BuyerCurrency;
  Quantity: number;
}
