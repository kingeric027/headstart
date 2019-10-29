import { Component, Input } from '@angular/core';
import { LineItem, ListLineItem } from '@ordercloud/angular-sdk';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { get as _get, map as _map, without as _without } from 'lodash';
import { OCMComponent } from '../base-component';
import { ListLineItemWithProduct } from 'marketplace';

@Component({
  templateUrl: './lineitem-table.component.html',
  styleUrls: ['./lineitem-table.component.scss'],
})
export class OCMLineitemTable extends OCMComponent {
  closeIcon = faTimes;
  @Input() lineItems: ListLineItem | ListLineItemWithProduct;
  @Input() readOnly: boolean;

  ngOnContextSet() {}

  removeLineItem(lineItemID: string) {
    this.context.currentOrder.removeFromCart(lineItemID);
  }

  toProductDetails(productID: string) {
    this.context.router.toProductDetails(productID);
  }

  changeQuantity(lineItemID: string, event: { qty: number, valid: boolean }) {
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
    const result = _map(images, (img) => {
      return img.Url.replace('{url}', host);
    });
    const filtered = _without(result, undefined);
    return filtered.length > 0 ? filtered[0] : 'http://placehold.it/300x300';
  }

  getLineItem(lineItemID: string): LineItem {
    return this.lineItems.Items.find(li => li.ID === lineItemID);
  }
}
