import { LineItem } from '@ordercloud/angular-sdk';
import { MarketplaceOrder } from '../../shopper-context';

// tentative models to be overriden by ordercloud sdk
export interface ShipmentPreference {
  ShipmentEstimateID: string;
  ShipmentMethodID: string;
}

export interface ShipmentEstimate {
  ID: string;
  ShipmentEstimateItems: ShipmentEstimateItem[];
  ShipmentMethods: ShipmentMethod[];
  SelectedShipMethodID: string;
  xp: any;
}

export interface ShipmentEstimateItem {
  LineItemID: string;
  Quantity: number;
}

export interface ShipmentMethod {
  ID: string;
  Name: string;
  EstimatedTransitDays: number;
  Cost: number;
  xp: any;
}

export interface OrderWorksheet {
  Order: MarketplaceOrder;
  LineItems: LineItem[];
  ShipmentEstimateResponse: ShipmentEstimateResponse;
  OrderCalculateResponse: OrderCalculateResponse;
}

export interface ShipmentEstimateResponse {
  ShipmentEstimates: ShipmentEstimate[];
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
