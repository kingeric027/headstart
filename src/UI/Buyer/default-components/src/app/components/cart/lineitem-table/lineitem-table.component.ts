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
    this.sortLineItems(this._lineItems);
    const liGroups = _groupBy(value, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(liGroups);
    this.sortLineItemGroups(this.liGroupedByShipFrom);
    this.setSupplierInfo(this.liGroupedByShipFrom);
  }
  @Input() orderType: OrderType;
  @Input() readOnly: boolean;
  @Input() hideStatus = false;
  suppliers: LineItemGroupSupplier[];
  liGroupedByShipFrom: MarketplaceLineItem[][];
  updatingLiIDs: string[] = [];
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

  async changeQuantity(lineItemID: string, event: QtyChangeEvent): Promise<void> {
    if (event.valid) {
      const li = this.getLineItem(lineItemID);
      li.Quantity = event.qty;
      const { ProductID, Specs, Quantity, xp } = li;
      //ACTIVATE SPINNER/DISABLE INPUT IF QTY BEING UPDATED
      this.updatingLiIDs.push(lineItemID);
      await this.context.order.cart.setQuantity({ProductID, Specs, Quantity, xp});
      //REMOVE SPINNER/ENABLE INPUT IF QTY NO LONGER BEING UPDATED
      this.updatingLiIDs.splice(this.updatingLiIDs.indexOf(lineItemID), 1);
    }
  }

  isQtyChanging(lineItemID: string): boolean {
    return this.updatingLiIDs.includes(lineItemID);
  }

  getImageUrl(lineItemID: string): string {
    const li = this.getLineItem(lineItemID);
    return li.xp.LineItemImageUrl;
  }

  getLineItem(lineItemID: string): MarketplaceLineItem {
    return this._lineItems.find(li => li.ID === lineItemID);
  }

  sortLineItems(lineItems: MarketplaceLineItem[]): void {
    this._lineItems = lineItems.sort((a, b) => {
      let nameA = a.Product.Name.toUpperCase(); // ignore upper and lowercase
      let nameB = b.Product.Name.toUpperCase(); // ignore upper and lowercase
      return nameA.localeCompare(nameB);
    });
  }

  sortLineItemGroups(liGroups: MarketplaceLineItem[][]): void {
    this.liGroupedByShipFrom = liGroups.sort((a, b) => {
      let nameA = a[0].ShipFromAddressID.toUpperCase(); // ignore upper and lowercase
      let nameB = b[0].ShipFromAddressID.toUpperCase(); // ignore upper and lowercase
      return nameA.localeCompare(nameB);
    })
  }

  hasReturnInfo(): boolean {
    return this._lineItems.some(li => !!li.xp?.LineItemReturnInfo);
  }

  getReturnReason(reasonCode: string): string {
    return ReturnReason[reasonCode];
  }
}
