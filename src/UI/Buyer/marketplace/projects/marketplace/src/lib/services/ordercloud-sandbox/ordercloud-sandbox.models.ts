import { Meta, Order, LineItemSpec, LineItem } from '@ordercloud/angular-sdk';

// tentative models to be overriden by ordercloud sdk
export interface ShipmentPreference {
    ProposedShipmentID: string;
    ProposedShipmentOptionID: string;
  }
  
  export interface ProposedShipment {
    ID: string;
    ProposedShipmentItems: ProposedShipmentItem[];
    ProposedShipmentOptions: ProposedShipmentOption[];
    SelectedProposedShipmentOptionID: string;
    xp: any;
}
  
  export interface ProposedShipmentItem {
    LineItemID: string;
    Quantity: number;
  }
  
  export interface ProposedShipmentOption {
    ID: string;
    Name: string;
    EstimatedDeliveryDays: number;
    Cost: number;
    xp: any;
  }

  export interface ProposedShipmentRatesResponse {
    ProposedShipments: ProposedShipment[]
  }

  export interface OrderCalculation {
    Order: Order;
    LineItems: LineItem[];
    ProposedShipmentRatesResponse: ProposedShipmentRatesResponse
    
    // todo type
    OrderCaculateResponse: any;
  }