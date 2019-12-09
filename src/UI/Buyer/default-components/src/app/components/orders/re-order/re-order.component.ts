import { Component, OnInit, Input } from '@angular/core';
import { forEach as _forEach } from 'lodash';

import { LineItem } from '@ordercloud/angular-sdk';
import { OrderReorderResponse, ShopperContextService } from 'marketplace';
import { ModalState } from 'src/app/models/modal-state.class';

@Component({
  templateUrl: './re-order.component.html',
  styleUrls: ['./re-order.component.scss'],
})
export class OCMReorder {
  reorderModal = ModalState.Closed;
  reorderResponse: OrderReorderResponse;
  message = { string: null, classType: null };

  constructor(private context: ShopperContextService) {}

  @Input() set orderID(value: string) {
    this.validateReorder(value);
  }

  async validateReorder(orderID: string): Promise<void> {
    this.reorderResponse = await this.context.orderHistory.validateReorder(orderID);
    this.updateMessage(this.reorderResponse);
  }
  
  openModal() {
    this.reorderModal = ModalState.Open;
  }

  updateMessage(response: OrderReorderResponse): void {
    if (response.InvalidLi.length && !response.ValidLi.length) {
      this.message.string = `None of the line items on this order are available for reorder.`;
      this.message.classType = 'danger';
      return;
    }
    if (response.InvalidLi.length && response.ValidLi.length) {
      this.message.string = `<strong>Warning</strong> The following line items are not available for reorder, clicking add to cart will <strong>only</strong> add valid line items.`;
      this.message.classType = 'warning';
      return;
    }
    this.message.string = `All line items are valid to reorder`;
    this.message.classType = 'success';
  }

  async addToCart(): Promise<void> {
    const items = this.reorderResponse.ValidLi.map(li => {
       return { ProductID: li.Product.ID, Quantity: li.Quantity, Specs: li.Specs };
    });
    await this.context.currentOrder.addManyToCart(items);
    this.reorderModal = ModalState.Closed;
  }
}
