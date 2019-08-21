import { Component, Output, Input, EventEmitter } from '@angular/core';
import { LineItem } from '@ordercloud/angular-sdk';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { get as _get } from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'shared-line-item-card',
  templateUrl: './line-item-card.component.html',
  styleUrls: ['./line-item-card.component.scss'],
})
export class LineItemCardComponent {
  closeIcon = faTimes;

  @Input() lineitem: LineItem;
  @Input() readOnly: boolean;
  @Input() quantityLimits: QuantityLimits;
  @Output() deleteLineItem = new EventEmitter<{ lineItemID: string }>();
  @Output() updateQuantity = new EventEmitter<{ lineItemID: string; quantity: number }>();

  constructor() {}

  protected removeLineItem() {
    this.deleteLineItem.emit({ lineItemID: this.lineitem.ID });
  }

  protected changeQuantity(qty: number) {
    this.lineitem.Quantity = qty;
    this.updateQuantity.emit({ lineItemID: this.lineitem.ID, quantity: qty });
  }

  getImageUrl() {
    return _get(this.lineitem, 'Product.xp.Images[0].Url', 'http://placehold.it/300x300');
  }
}
