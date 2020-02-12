import { Component, Input } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { Address, LineItem, Supplier } from '@ordercloud/angular-sdk';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService } from 'marketplace';
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
    this.getSupplierInfo(this.liGroupedByShipFrom);
  }
  @Input() readOnly: boolean;
  supplierInfo: Supplier[] = [];
  supplierAddresses: Address[] = [];
  liGroupedByShipFrom: LineItem[][];
  liGroups: any;
  _lineItems = [];
  constructor(private context: ShopperContextService) { }

  async getSupplierInfo(liGroupedByShipFrom: LineItem[][]) {
    this.supplierInfo = await this.context.orderHistory.getSupplierInfo(liGroupedByShipFrom);
    this.supplierAddresses = await this.context.orderHistory.getSupplierAddresses(liGroupedByShipFrom);
  }

  removeLineItem(lineItemID: string) {
    this.context.currentOrder.removeFromCart(lineItemID);
  }

  toProductDetails(productID: string) {
    this.context.router.toProductDetails(productID);
  }

  changeQuantity(lineItemID: string, event: { qty: number; valid: boolean }) {
    if (event.valid) {
      this.getLineItem(lineItemID).Quantity = event.qty;
      this.context.currentOrder.setQuantityInCart(lineItemID, event.qty);
    }
  }

  getImageUrl(lineItemID: string) {
    const li = this.getLineItem(lineItemID);
    if (li && li.Product) {
      return getPrimaryImageUrl(li.Product);
    }
  }

  getLineItem(lineItemID: string): LineItem {
    return this._lineItems.find(li => li.ID === lineItemID);
  }
}
