import { Component, OnInit, Inject, Input } from '@angular/core';
import { CheckoutSectionBaseComponent } from '@app-buyer/checkout/components/checkout-section-base/checkout-section-base.component';
import { AppStateService, CartService } from '@app-buyer/shared';
import { Order, ListPayment, ListLineItem, OcOrderService } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { AppPaymentService } from '@app-buyer/shared/services/app-payment-service/app-payment.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';
import { Router } from '@angular/router';

@Component({
  selector: 'checkout-confirm',
  templateUrl: './checkout-confirm.component.html',
  styleUrls: ['./checkout-confirm.component.scss'],
})
export class CheckoutConfirmComponent extends CheckoutSectionBaseComponent implements OnInit {
  form: FormGroup;
  order: Order;
  payments$: Observable<ListPayment>;
  lineItems: ListLineItem;
  @Input() isSubmittingOrder: boolean;

  constructor(
    private appStateService: AppStateService,
    private appPaymentService: AppPaymentService,
    private cartService: CartService,
    private formBuilder: FormBuilder,
    private ocOrderService: OcOrderService,
    private router: Router,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    super();
  }

  async ngOnInit() {
    if (!this.appConfig.anonymousShoppingEnabled) {
      this.form = this.formBuilder.group({ comments: '' });
    }
    this.order = this.appStateService.orderSubject.value;
    this.payments$ = this.appPaymentService.getPayments('outgoing', this.order.ID);
    this.lineItems = await this.cartService.listAllItems(this.order.ID);
  }

  saveCommentsAndSubmitOrder() {
    if (this.isSubmittingOrder) {
      return;
    }
    this.isSubmittingOrder = true;
    this.ocOrderService
      .Patch('outgoing', this.order.ID, {
        Comments: this.form.get('comments').value,
      })
      .subscribe((order) => {
        this.appStateService.orderSubject.next(order);
        this.continue.emit();
      });
  }

  toProductDetails(productID: string) {
    this.router.navigateByUrl(`/products/${productID}`);
  }
}
