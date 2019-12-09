import { Component, OnInit } from '@angular/core';
import { Order, ListPayment, ListLineItem } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './checkout-confirm.component.html',
  styleUrls: ['./checkout-confirm.component.scss'],
})
export class OCMCheckoutConfirm implements OnInit {
  form: FormGroup;
  order: Order;
  payments: ListPayment;
  lineItems: ListLineItem;
  anonEnabled: boolean;
  isSubmittingOrder = false; // prevent double-click submits

  constructor(private context: ShopperContextService) {}

  async ngOnInit() {
    this.form = new FormGroup({ comments: new FormControl('') });
    this.anonEnabled = this.context.appSettings.anonymousShoppingEnabled;
    this.order = this.context.currentOrder.get();
    this.lineItems = this.context.currentOrder.getLineItems();
    this.payments = await this.context.currentOrder.listPayments();
  }

  async saveCommentsAndSubmitOrder() {
    this.isSubmittingOrder = true;
    const Comments = this.form.get('comments').value;
    const orderID = this.context.currentOrder.get().ID;
    await this.context.currentOrder.patch({ Comments });
    try {
      await this.context.currentOrder.submit();
    } catch (ex) {
      throw new Error(ex);
    }

    // todo: "Order Submitted Successfully" message
    this.context.router.toMyOrderDetails(orderID);
  }
}
