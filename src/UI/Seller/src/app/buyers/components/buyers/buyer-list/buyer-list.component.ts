import { Component, ChangeDetectorRef } from '@angular/core';
import { Promotion } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { PromotionService } from '@app-seller/shared/services/promotion/promotion.service';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'buyer-list',
  templateUrl: './buyer-list.component.html',
  styleUrls: ['./buyer-list.component.scss'],
})
export class BuyerListComponent extends ResourceCrudComponent<Promotion> {
  constructor(
    private buyerService: BuyerService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute
  ) {
    super(changeDetectorRef, buyerService, router, activatedRoute);
  }

  route = 'buyer';
}
