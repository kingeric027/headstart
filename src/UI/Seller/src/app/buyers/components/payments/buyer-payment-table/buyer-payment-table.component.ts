import { Component, ChangeDetectorRef } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { CreditCard } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { BuyerLocationService } from '@app-seller/shared/services/buyer/buyer-location.service';
import { BuyerPaymentService } from '@app-seller/shared/services/buyer/buyer-payment.service';

@Component({
  selector: 'app-buyer-payment-table',
  templateUrl: './buyer-payment-table.component.html',
  styleUrls: ['./buyer-payment-table.component.scss'],
})
export class BuyerPaymentTableComponent extends ResourceCrudComponent<CreditCard> {
  constructor(
    private buyerPaymentService: BuyerPaymentService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private buyerService: BuyerService
  ) {
    super(changeDetectorRef, buyerPaymentService, router, activatedroute);
  }
}
