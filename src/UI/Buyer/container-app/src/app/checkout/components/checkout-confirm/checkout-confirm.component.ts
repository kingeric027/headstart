import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Order, ListPayment, ListLineItem, OcOrderService } from '@ordercloud/angular-sdk';
import { AppPaymentService } from 'src/app/shared/services/app-payment/app-payment.service';
import { FormGroup, FormControl } from '@angular/forms';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'checkout-confirm',
  templateUrl: './checkout-confirm.component.html',
  styleUrls: ['./checkout-confirm.component.scss'],
})
export class CheckoutConfirmComponent implements OnInit {
  form: FormGroup;
  order: Order;
  payments: ListPayment;
  lineItems: ListLineItem;
  anonEnabled: boolean;
  @Input() isSubmittingOrder: boolean;
  @Output() continue = new EventEmitter();

  constructor(
    private appPaymentService: AppPaymentService,
    private ocOrderService: OcOrderService,
    public context: ShopperContextService //used in template
  ) {}

  async ngOnInit() {
    this.anonEnabled = this.context.appSettings.anonymousShoppingEnabled;
    this.order = this.context.currentOrder.order;
    this.lineItems = this.context.currentOrder.lineItems;
    if (!this.anonEnabled) {
      this.form = new FormGroup({ comments: new FormControl('') });
    }
    this.payments = await this.appPaymentService.ListPaymentsOnOrder(this.order.ID);
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
