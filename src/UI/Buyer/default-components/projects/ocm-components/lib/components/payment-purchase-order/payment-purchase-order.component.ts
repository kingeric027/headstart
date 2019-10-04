import { Component, OnChanges, SimpleChanges, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { get as _get } from 'lodash';
import { Payment, Order } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './payment-purchase-order.component.html',
  styleUrls: ['./payment-purchase-order.component.scss'],
})
export class OCMPaymentPurchaseOrder extends OCMComponent implements OnInit, OnChanges {
  order: Order;
  form: FormGroup;
  @Input() payment: Payment;
  @Output() continue = new EventEmitter();

  ngOnInit() {
    this.form = new FormGroup({ PONumber: new FormControl('') });
  }

  ngOnContextSet() {
    this.form.setValue( { PONumber: this.getPONumber(this.payment) });
    this.order = this.context.currentOrder.get();
  }

  getPONumber(payment: Payment): string {
    if (!payment || !payment.xp || !payment.xp.PONumber) return '';
    return payment.xp.PONumber;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.payment) {
      this.payment = changes.payment.currentValue;
      // set form
      if (changes.payment.firstChange) {
        this.form.setValue( { PONumber: this.getPONumber(this.payment) });
      }

      // validate payment
      if (!this.paymentValid()) {
        this.createNewPayment();
      }
    }
  }

  createNewPayment() {
    const PONumber = this.getPONumber(this.payment);
    this.payment = { Type: 'PurchaseOrder', xp: { PONumber } };
    this.context.currentOrder.createPayment(this.payment);
  }

  updatePONumber() {}

  validateAndContinue() {
    if (this.paymentValid()) {
      this.continue.emit();
    }
  }

  paymentValid(): boolean {
    return !!this.payment && this.payment.Amount === this.order.Total;
  }
}
