export interface SupportedRates {
  Currency: string;
  Symbol: string;
  Name: string;
  Icon?: string;
}

export enum SupportedCurrencies {
  USD = 'USD',
  CAD = 'CAD',
  EUR = 'EUR',
}