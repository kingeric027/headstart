import { Component, Input } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService, LineItemGroupSupplier, OrderType, MarketplaceLineItem } from 'marketplace';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';
import { CurrentUserService } from 'marketplace/projects/marketplace/src/lib/services/current-user/current-user.service';

@Component({
  templateUrl: './lineitem-table.component.html',
  styleUrls: ['./lineitem-table.component.scss'],
})
export class OCMLineitemTable {
  closeIcon = faTimes;
  @Input() set lineItems(value: MarketplaceLineItem[]) {
    this._lineItems = value;
    this.liGroups = _groupBy(value, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(this.liGroups);
    this.setSupplierInfo(this.liGroupedByShipFrom);
  }
  @Input() orderType: OrderType;
  @Input() readOnly: boolean;
  suppliers: LineItemGroupSupplier[];
  liGroupedByShipFrom: MarketplaceLineItem[][];
  liGroups: any;
  _lineItems = [];
  _orderCurrency: string;

  constructor(private context: ShopperContextService) { 
    const currentUser = this.context.currentUser.get();
    // Using `|| "USD"` for fallback right now in case there's bad data without the xp value.
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === "BuyerLocation")[0].xp?.Currency || "USD";
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

  getLineItem(lineItemID: string): MarketplaceLineItem {
    return this._lineItems.find(li => li.ID === lineItemID);
  }

  hasReturnInfo() {
    return this.liGroupedByShipFrom.find(liGroup => liGroup.find(li => li.xp?.LineItemReturnInfo));
  }

}
