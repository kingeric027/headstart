export interface ExchangeRates {
  BaseSymbol: string;
  Rates: Rates[];
}

export interface Rates {
  Symbol: string;
  Name: string;
  Rate: number;
}
