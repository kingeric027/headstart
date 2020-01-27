import { Component, Input, OnInit } from '@angular/core';
import { LineItem } from '@ordercloud/angular-sdk';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { get as _get, map as _map, without as _without } from 'lodash';
import { ShopperContextService, LineItemWithProduct, ShippingRate } from 'marketplace';
import { FormGroup, FormControl } from '@angular/forms';

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

  // TODO - we need a unified getImageUrl() function
  getImageUrl(lineItemID: string) {
    const li = this.getLineItem(lineItemID);
    const host = 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product';
    const images = li.Product.xp.Images || [];
    const result = _map(images, img => {
      return img.Url.replace('{u}', host);
    });
    const filtered = _without(result, undefined);
    return filtered.length > 0 ? filtered[0] : 'http://placehold.it/300x300';
  }

  getLineItem(lineItemID: string): LineItem {
    return this.lineItems.find(li => li.ID === lineItemID);
  }

  getShipFromAddressID(): string {
    return this.lineItems[0].ShipFromAddressID; 
  }
}
