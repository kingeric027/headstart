import { LineItem } from '@ordercloud/angular-sdk';
import { MarketplaceOrder } from '../../shopper-context';

// tentative models to be overriden by ordercloud sdk
export interface ShipMethodSelection {
  ShipEstimateID: string;
  ShipMethodID: string;
}

export interface ShipEstimate {
  ID: string;
  ShipEstimateItems: ShipEstimateItem[];
  ShipMethods: ShipMethod[];
  SelectedShipMethodID: string;
  xp: any;
}

export interface ShipEstimateItem {
  LineItemID: string;
  Quantity: number;
}

export interface ShipMethod {
  ID: string;
  Name: string;
  EstimatedTransitDays: number;
  Cost: number;
  xp: any;
}

export interface OrderWorksheet {
  Order: MarketplaceOrder;
  LineItems: LineItem[];
  ShipEstimateResponse: ShipEstimateResponse;
  OrderCalculateResponse: OrderCalculateResponse;
}

export interface ShipEstimateResponse {
  ShipEstimates: ShipEstimate[];
}

export interface OrderCalculateResponse {
  LineItemOverrides: LineItemOverride[];
  ShippingTotal: number;
  TaxTotal: number;
  xp: any;
}

export interface LineItemOverride {
  LineItemID: string;
  UnitPrice: number;
}
