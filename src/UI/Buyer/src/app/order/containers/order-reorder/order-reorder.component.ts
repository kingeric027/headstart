import { Component, OnInit, Input } from '@angular/core';
import { forEach as _forEach } from 'lodash';

import { CartService, AppReorderService } from 'src/app/shared';
import { OrderReorderResponse } from 'src/app/shared/services/reorder/reorder.interface';
import { LineItem } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'order-reorder',
  templateUrl: './order-reorder.component.html',
  styleUrls: ['./order-reorder.component.scss'],
})
export class OrderReorderComponent implements OnInit {
  @Input() orderID: string;
  reorderModalOpen = false;
  reorderResponse: OrderReorderResponse;
  message = { string: null, classType: null };

  constructor(
    private appReorderService: AppReorderService,
    private cartService: CartService,
    public context: ShopperContextService // used in template
  ) {}

  async ngOnInit() {
    if (this.orderID) {
      this.reorderResponse = await this.appReorderService.validateReorder(this.orderID);
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
    this.reorderModalOpen = true;
  }

  addToCart() {
    _forEach(this.reorderResponse.ValidLi, async (li: LineItem) => {
      if (!li) return;
      li = { ProductID: li.Product.ID, Quantity: li.Quantity, Specs: li.Specs };
      await this.cartService.addToCart(li);
    });
    this.reorderModalOpen = false;
  }
}
