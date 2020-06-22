import { Component, Input } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService, LineItemGroupSupplier, OrderType } from 'marketplace';
import { MarketplaceLineItem } from 'marketplace-javascript-sdk';
import { QtyChangeEvent } from '../../products/quantity-input/quantity-input.component';
import { ReturnReason } from '../../orders/order-return/order-return-table/return-reason-enum';

@Component({
  templateUrl: './lineitem-table.component.html',
  styleUrls: ['./lineitem-table.component.scss'],
})
export class OCMLineitemTable {
  closeIcon = faTimes;
  @Input() set lineItems(value: MarketplaceLineItem[]) {
    this._lineItems = value;
    const liGroups = _groupBy(value, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(liGroups);
    this.setSupplierInfo(this.liGroupedByShipFrom);
  }
  @Input() orderType: OrderType;
  @Input() readOnly: boolean;
  suppliers: LineItemGroupSupplier[];
  liGroupedByShipFrom: MarketplaceLineItem[][];
  _lineItems = [];
  _orderCurrency: string;

  constructor(private context: ShopperContextService) { 
    this._orderCurrency = this.context.currentUser.get().Currency;
  }

  async setSupplierInfo(liGroups: MarketplaceLineItem[][]): Promise<void> {
    this.suppliers = await this.context.orderHistory.getLineItemSuppliers(liGroups);
  }

  removeLineItem(lineItemID: string): void {
    this.context.order.cart.remove(lineItemID);
  }

  toProductDetails(productID: string): void {
    this.context.router.toProductDetails(productID);
  }

  changeQuantity(lineItemID: string, event: QtyChangeEvent): void {
    if (event.valid) {
      const li = this.getLineItem(lineItemID);
      li.Quantity = event.qty;
      const { ProductID, Specs, Quantity } = li;
      this.context.order.cart.add({ProductID, Specs, Quantity});
    }
  }

  getImageUrl(lineItemID: string): string {
    const li = this.getLineItem(lineItemID);
    return li.xp.LineItemImageUrl;
  }

  getLineItem(lineItemID: string): MarketplaceLineItem {
    return this._lineItems.find(li => li.ID === lineItemID);
  }

  hasReturnInfo(): boolean {
    return this._lineItems.some(li => !!li.xp?.LineItemReturnInfo);
  }

  getReturnReason(reasonCode: string): string {
    return ReturnReason[reasonCode];
  }
}
