import { Component, OnInit, Input } from '@angular/core';
import { forEach as _forEach } from 'lodash';

import { LineItem } from '@ordercloud/angular-sdk';
import { OrderReorderResponse } from 'marketplace';
import { ModalState } from 'src/app/models/modal-state.class';
import { OCMComponent } from '../../base-component';

@Component({
  templateUrl: './re-order.component.html',
  styleUrls: ['./re-order.component.scss'],
})
export class OCMReorder extends OCMComponent {
  @Input() orderID: string;
  reorderModal = ModalState.Closed;
  reorderResponse: OrderReorderResponse;
  message = { string: null, classType: null };

  async ngOnContextSet() {
    if (this.orderID) {
      this.reorderResponse = await this.context.orderHistory.validateReorder(this.orderID);
      this.updateMessage(this.reorderResponse);
    } else {
      throw new Error('Needs Order ID');
    }
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

  orderReorder() {
    this.reorderModal = ModalState.Open;
  }

  addToCart() {
    _forEach(this.reorderResponse.ValidLi, async (li: LineItem) => {
      if (!li) return;
      li = { ProductID: li.Product.ID, Quantity: li.Quantity, Specs: li.Specs };
      await this.context.currentOrder.addToCart(li);
    });
    this.reorderModal = ModalState.Closed;
  }
}
