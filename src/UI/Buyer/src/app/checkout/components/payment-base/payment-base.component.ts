import { Input, EventEmitter, Output } from '@angular/core';
import { Payment, Order, PartialPayment } from '@ordercloud/angular-sdk';
import { PaymentMethod } from 'src/app/shared/models/payment-method.enum';
import { FormGroup } from '@angular/forms';

export class PaymentBaseComponent {
  @Input() order: Order;
  @Input() payment: Payment;
  @Input() paymentMethod: PaymentMethod;
  @Output() paymentCreated = new EventEmitter<Payment>();
  @Output()
  paymentPatched = new EventEmitter<{
    paymentID: string;
    payment: PartialPayment;
  }>();
  @Output() continue = new EventEmitter();
  form: FormGroup;

  constructor() {}

  paymentValid() {
    return !!this.payment && this.payment.Amount === this.order.Total;
  }
}
