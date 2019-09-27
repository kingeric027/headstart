import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { OcPaymentService, Payment, PartialPayment } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import { CurrentOrderService } from 'src/app/shared';
import { PaymentMethod } from 'src/app/shared/models/payment-method.enum';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss'],
})
export class CheckoutPaymentComponent implements OnInit {
  constructor(
    private currentOrder: CurrentOrderService,
    private ocPaymentService: OcPaymentService,
    private context: ShopperContextService
  ) {}

  @Input() isAnon: boolean;
  @Output() continue = new EventEmitter();

  readonly order = this.currentOrder.order;
  form: FormGroup;
  availablePaymentMethods = this.context.appSettings.availablePaymentMethods;
  selectedPaymentMethod: PaymentMethod;
  existingPayment: Payment;

  async ngOnInit() {
    this.form = new FormGroup({
      selectedPaymentMethod: new FormControl({ value: '', disabled: this.availablePaymentMethods.length === 1 }),
    });
    await this.initializePaymentMethod();
  }

  async initializePaymentMethod(): Promise<void> {
    if (this.availablePaymentMethods.length === 1) {
      this.selectedPaymentMethod = this.availablePaymentMethods[0];
    }
    const payments = await this.ocPaymentService.List('outgoing', this.order.ID).toPromise();
    if (payments.Items && payments.Items.length > 0) {
      this.existingPayment = payments.Items[0];
    } else {
      this.existingPayment = null;
    }

    if (this.existingPayment) {
      await this.selectPaymentMethod(this.existingPayment.Type as PaymentMethod);
    } else {
      await this.selectPaymentMethod(this.availablePaymentMethods[0]);
    }
  }

  async selectPaymentMethod(method: PaymentMethod): Promise<void> {
    if (method) {
      this.form.controls['selectedPaymentMethod'].setValue(method);
    }
    this.selectedPaymentMethod = this.form.get('selectedPaymentMethod').value;
    if (this.selectedPaymentMethod !== PaymentMethod.SpendingAccount && this.existingPayment && this.existingPayment.SpendingAccountID) {
      this.existingPayment = null;
      await this.deleteExistingPayments();
    }
  }

  onContinueClicked() {
    this.continue.emit();
  }

  async createPayment(payment: Payment) {
    await this.deleteExistingPayments();
    this.existingPayment = await this.ocPaymentService.Create('outgoing', this.order.ID, payment).toPromise();
  }

  private async deleteExistingPayments(): Promise<any[]> {
    const payments = await this.ocPaymentService.List('outgoing', this.order.ID).toPromise();
    const deleteAll = payments.Items.map((payment) => this.ocPaymentService.Delete('outgoing', this.order.ID, payment.ID).toPromise());
    return Promise.all(deleteAll);
  }

  async patchPayment({ paymentID, payment }: { paymentID: string; payment: PartialPayment }) {
    this.existingPayment = await this.ocPaymentService.Patch('outgoing', this.order.ID, paymentID, payment).toPromise();
  }
}
