import { Component, Output, EventEmitter } from '@angular/core';
import { Payment, Order } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss'],
})
export class OCMCheckoutPayment extends OCMComponent {
  @Output() continue = new EventEmitter();

  order: Order;
  isAnon: boolean;
  availablePaymentMethods = ['PurchaseOrder', 'SpendingAccount', 'CreditCard'];
  selectedPaymentMethod: string;
  existingPayment: Payment;
  form = new FormGroup({
    selectedPaymentMethod: new FormControl({ value: '', disabled: this.availablePaymentMethods.length === 1 }),
  });

  async ngOnContextSet() {
    this.order = this.context.currentOrder.get();
    this.isAnon = this.context.currentUser.isAnonymous;
    await this.initializePaymentMethod();
  }

  async initializePaymentMethod(): Promise<void> {
    if (this.availablePaymentMethods.length === 1) {
      this.selectedPaymentMethod = this.availablePaymentMethods[0];
    }
    const payments = await this.context.currentOrder.listPayments();
    if (payments.Items && payments.Items.length > 0) {
      this.existingPayment = payments.Items[0]; // TODO - we will need to support multiple payments
    } else {
      this.existingPayment = null;
    }

    if (this.existingPayment) {
      await this.selectPaymentMethod(this.existingPayment.Type);
    } else {
      await this.selectPaymentMethod(this.availablePaymentMethods[0]);
    }
  }

  async selectPaymentMethod(method: string): Promise<void> {
    if (method) {
      this.form.controls.selectedPaymentMethod.setValue(method);
    }
    this.selectedPaymentMethod = this.form.get('selectedPaymentMethod').value;
    if (this.selectedPaymentMethod !== 'SpendingAccount' && this.existingPayment && this.existingPayment.SpendingAccountID) {
      this.existingPayment = null;
    }
  }

  onContinueClicked() {
    this.continue.emit();
  }
}
