import { Component, Input, OnInit } from '@angular/core';
import { LineItem } from '@ordercloud/angular-sdk';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { ShopperContextService, LineItemWithProduct, ShippingRate } from 'marketplace';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';

@Component({
  templateUrl: './lineitem-table.component.html',
  styleUrls: ['./lineitem-table.component.scss'],
})
export class OCMLineitemTable implements OnInit {
  closeIcon = faTimes;
  @Input() lineItems: LineItem[] | LineItemWithProduct[] = [];
  @Input() readOnly: boolean;
 
  constructor(private context: ShopperContextService) {}

  ngOnInit() {}

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
    return getPrimaryImageUrl(li.Product);
  }

  getLineItem(lineItemID: string): LineItem {
    return this.lineItems.find(li => li.ID === lineItemID);
  }

  getShipFromAddressID(): string {
    return this.lineItems[0].ShipFromAddressID; 
  }
}
