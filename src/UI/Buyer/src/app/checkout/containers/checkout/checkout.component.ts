import { Component, OnInit, ViewChild } from '@angular/core';
import { throwError } from 'rxjs';
import { Order, OcOrderService } from '@ordercloud/angular-sdk';
import { CurrentOrderService } from 'src/app/shared';
import { faCheck } from '@fortawesome/free-solid-svg-icons';
import { NgbAccordion } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { AppErrorHandler } from 'src/app/config/error-handling.config';
import { CurrentUserService } from 'src/app/shared/services/current-user/current-user.service';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'checkout-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements OnInit {
  @ViewChild('acc', { static: false }) public accordian: NgbAccordion;
  order: Order;
  isAnon: boolean;
  isSubmittingOrder = false;
  currentPanel: string;
  faCheck = faCheck;
  sections: any = [
    {
      id: 'login',
      valid: false,
    },
    {
      id: 'shippingAddress',
      valid: false,
    },
    {
      id: 'billingAddress',
      valid: false,
    },
    {
      id: 'payment',
      valid: false,
    },
    {
      id: 'confirm',
      valid: false,
    },
  ];

  constructor(
    private currentUser: CurrentUserService,
    private currentOrder: CurrentOrderService,
    private ocOrderService: OcOrderService,
    private router: Router,
    private appErrorHandler: AppErrorHandler,
    public context: ShopperContextService
  ) {}

  ngOnInit() {
    this.currentOrder.onOrderChange((order) => (this.order = order));
    this.isAnon = this.currentUser.isAnonymous;
    this.currentPanel = this.isAnon ? 'login' : 'shippingAddress';
    this.setValidation('login', !this.isAnon);
  }

  getValidation(id: string) {
    return this.sections.find((x) => x.id === id).valid;
  }

  setValidation(id: string, value: boolean) {
    this.sections.find((x) => x.id === id).valid = value;
  }

  toSection(id: string) {
    const prevIdx = Math.max(this.sections.findIndex((x) => x.id === id) - 1, 0);
    const prev = this.sections[prevIdx].id;
    this.setValidation(prev, true);
    this.accordian.toggle(id);
  }

  async submitOrder() {
    this.isSubmittingOrder = true;
    const orderID = this.currentOrder.order.ID;
    const order = await this.ocOrderService.Get('outgoing', orderID).toPromise();
    if (order.IsSubmitted) {
      return throwError({ message: 'Order has already been submitted' });
    }
    try {
      await this.ocOrderService.Submit('outgoing', orderID).toPromise();
    } catch (ex) {
      this.isSubmittingOrder = false;
      this.appErrorHandler.displayError(ex);
    }
    this.router.navigateByUrl(`order-confirmation/${orderID}`);
    this.currentOrder.reset();
  }

  beforeChange($event) {
    if (this.currentPanel === $event.panelId) {
      return $event.preventDefault();
    }

    // Only allow a section to open if all previous sections are valid
    for (const section of this.sections) {
      if (section.id === $event.panelId) {
        break;
      }

      if (!section.valid) {
        return $event.preventDefault();
      }
    }
    this.currentPanel = $event.panelId;
  }
}
