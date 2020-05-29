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

  changeQuantity(lineItemID: string, event: { qty: number; valid: boolean }): void {
    if (event.valid) {
      const li = this.getLineItem(lineItemID);
      li.Quantity = event.qty;
      const { ProductID, Specs, Quantity, ...rest } = li;
      this.context.order.cart.add({ProductID, Specs, Quantity});
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
