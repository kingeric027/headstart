import { Component, OnInit, Input } from '@angular/core';
import { CheckoutSectionBaseComponent } from 'src/app/checkout/components/checkout-section-base/checkout-section-base.component';
import { Order, ListPayment, ListLineItem, OcOrderService } from '@ordercloud/angular-sdk';
import { AppPaymentService } from 'src/app/shared/services/app-payment/app-payment.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'checkout-confirm',
  templateUrl: './checkout-confirm.component.html',
  styleUrls: ['./checkout-confirm.component.scss'],
})
export class CheckoutConfirmComponent extends CheckoutSectionBaseComponent implements OnInit {
  form: FormGroup;
  order: Order;
  payments: ListPayment;
  lineItems: ListLineItem;
  anonEnabled: boolean;
  @Input() isSubmittingOrder: boolean;

  constructor(
    private appPaymentService: AppPaymentService,
    private formBuilder: FormBuilder,
    private ocOrderService: OcOrderService,
    public context: ShopperContextService //used in template
  ) {
    super();
  }

  async ngOnInit() {
    this.anonEnabled = this.context.appSettings.anonymousShoppingEnabled;
    this.order = this.context.currentOrder.order;
    this.lineItems = this.context.currentOrder.lineItems;
    if (!this.anonEnabled) {
      this.form = this.formBuilder.group({ comments: '' });
    }
    this.payments = await this.appPaymentService.getPayments('outgoing', this.order.ID);
  }

  async saveCommentsAndSubmitOrder() {
    if (this.isSubmittingOrder) {
      return;
    }
    this.isSubmittingOrder = true;
    const Comments = this.form.get('comments').value;
    const order = await this.ocOrderService.Patch('outgoing', this.order.ID, { Comments }).toPromise();
    this.context.currentOrder.order = order;
    this.continue.emit();
  }
}
