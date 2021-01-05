import { MarketplaceLineItem } from "@ordercloud/headstart-sdk";
import { LineItem, Sortable } from "ordercloud-javascript-sdk";

export interface OrderSummaryMeta {
    StandardLineItems: MarketplaceLineItem[]
    POLineItems: MarketplaceLineItem[]
    StandardLineItemCount: number
    POLineItemCount: number
  
    ShouldHideShippingAndText: boolean
    ShippingAndTaxOverrideText: string
  
    // with no purchase order these are displayed as the whole order
    CreditCardDisplaySubtotal: number
    ShippingCost: number
    TaxCost: number
    CreditCardTotal: number
    DiscountTotal: number
  
    POSubtotal: number
    POShippingCost: number
    POTotal: number
    OrderTotal: number
  }

export enum OrderAddressType {
  Billing = 'Billing',
  Shipping = 'Shipping',
}

export enum OrderStatus {
  AllSubmitted = '!Unsubmitted',
  AwaitingApproval = 'AwaitingApproval',
  ChangesRequested = 'ChangesRequested',
  Open = 'Open',
  Completed = 'Completed',
  Canceled = 'Canceled',
}

export interface OrderFilters {
  page?: number
  sortBy?: Sortable<'Me.ListOrders'>
  search?: string
  showOnlyFavorites?: boolean
  status?: OrderStatus
  /**
   * mm-dd-yyyy
   */
  fromDate?: string
  /**
   * mm-dd-yyyy
   */
  toDate?: string
  location?: string
}


export enum OrderViewContext {
  MyOrders = 'MyOrders',
  Approve = 'Approve',
  Location = 'Location',
}

export interface OrderReorderResponse {
  ValidLi: Array<LineItem>
  InvalidLi: Array<LineItem>
}