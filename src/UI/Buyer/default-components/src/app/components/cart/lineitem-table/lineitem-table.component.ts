import { Component, Input } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { LineItem } from '@ordercloud/angular-sdk';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService, LineItemGroupSupplier, OrderType } from 'marketplace';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';

@Component({
  templateUrl: './lineitem-table.component.html',
  styleUrls: ['./lineitem-table.component.scss'],
})
export class OCMLineitemTable {
  closeIcon = faTimes;
  @Input() set lineItems(value: LineItem[]) {
    this._lineItems = value;
    this.liGroups = _groupBy(value, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(this.liGroups);
    this.setSupplierInfo(this.liGroupedByShipFrom);
  }
  @Input() orderType: OrderType;
  @Input() readOnly: boolean;
  suppliers: LineItemGroupSupplier[];
  liGroupedByShipFrom: LineItem[][];
  liGroups: any;
  _lineItems = [];

  constructor(private context: ShopperContextService) { }

  async setSupplierInfo(liGroups: LineItem[][]): Promise<void> {
    this.suppliers = await this.context.orderHistory.getLineItemSuppliers(liGroups);
  }

  removeLineItem(lineItemID: string): void {
    this.context.order.cart.remove(lineItemID);
  }

  toProductDetails(productID: string): void {
    this.context.router.toProductDetails(productID);
  }

  changeQuantity(lineItemID: string, event: { qty: number; valid: boolean }): void {
    if (event.valid) {
      this.getLineItem(lineItemID).Quantity = event.qty;
      this.context.order.cart.setQuantity(lineItemID, event.qty);
    }
  }

  getImageUrl(lineItemID: string): string {
    const li = this.getLineItem(lineItemID);
    return getPrimaryImageUrl(li?.Product);
  }

  getLineItem(lineItemID: string): LineItem {
    return this._lineItems.find(li => li.ID === lineItemID);
  }

  isQuoteOrder(): boolean {
    return this.orderType === OrderType.Quote;
  }

}
