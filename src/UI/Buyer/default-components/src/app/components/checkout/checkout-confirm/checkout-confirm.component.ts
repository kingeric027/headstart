import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { ListPayment, ListLineItem } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import { MarketplaceOrder } from 'marketplace-javascript-sdk';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './checkout-confirm.component.html',
  styleUrls: ['./checkout-confirm.component.scss'],
})
export class OCMCheckoutConfirm implements OnInit {
  form: FormGroup;
  isSubmittingOrder = false; // prevent double-click submits

  @Input() isAnon: boolean;
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListLineItem;
  @Input() payments: ListPayment;
  @Output() submitOrderWithComment = new EventEmitter<string>();

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.form = new FormGroup({ comments: new FormControl('') });
  }

  async saveCommentsAndSubmitOrder(): Promise<void> {
    await this.context.order.checkout.initializeHasReturn();
    this.isSubmittingOrder = true;
    const Comments = this.form.get('comments').value;
    this.submitOrderWithComment.emit(Comments);
  }
}
