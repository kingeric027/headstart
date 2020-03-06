import { Order } from '@ordercloud/angular-sdk'

export type MarketplaceOrder = Order<MarketplaceOrderXp>;

export enum OrderType {
    Quote = 'Quote',
    Standard = 'Standard'
}

export interface MarketplaceOrderXp {
    AvalaraTaxTransactionCode: string;
    OrderType: OrderType;
    QuoteOrderInfo: QuoteOrderInfo;
}

export interface QuoteOrderInfo {
    FirstName: string;
    LastName: string;
    Phone: string;
    Email: string;
    Comments?: string;
}