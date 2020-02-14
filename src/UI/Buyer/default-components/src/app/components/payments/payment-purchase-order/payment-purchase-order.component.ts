import { Component, OnChanges, SimpleChanges, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { get as _get } from 'lodash';
import { Payment } from '@ordercloud/angular-sdk';
import { ShopperContextService, MarketplaceOrder } from 'marketplace';

@Component({
  templateUrl: './payment-purchase-order.component.html',
  styleUrls: ['./payment-purchase-order.component.scss'],
})
export class OCMPaymentPurchaseOrder implements OnInit {
  order: MarketplaceOrder;
  form: FormGroup;
  _payment: Payment;
  @Output() continue = new EventEmitter();

  constructor(private context: ShopperContextService) {}

  @Input() set payment(value: Payment) {
    this._payment = value;
    this.form.setValue({ PONumber: this.getPONumber(value) });
  }

  ngOnInit() {
    this.order = this.context.currentOrder.get();
    this.form = new FormGroup({ PONumber: new FormControl('') });
  }

  getPONumber(payment: Payment): string {
    if (!payment || !payment.xp || !payment.xp.PONumber) return '';
    return payment?.xp.PONumber;
  }

  async saveAndContinue() {
    const PONumber = this.form.value.PONumber;
    this._payment = { Type: 'PurchaseOrder', Amount: this.order.Total, xp: { PONumber } };
    await this.context.currentOrder.createPayment(this._payment);
    if (this.paymentValid()) {
      this.continue.emit();
    }
  }

  paymentValid(): boolean {
    return !!this._payment && this._payment.Amount === this.order.Total;
  }
}
