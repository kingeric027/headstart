import { Component, ChangeDetectorRef } from '@angular/core';
import { Promotion } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { PromotionService } from '@app-seller/shared/services/promotion/promotion.service';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'buyer-table',
  templateUrl: './buyer-table.component.html',
  styleUrls: ['./buyer-table.component.scss'],
})
export class BuyerTableComponent extends ResourceCrudComponent<Promotion> {
  constructor(
    private buyerService: BuyerService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute
  ) {
    super(changeDetectorRef, buyerService, router, activatedRoute);
  }

  subResourceList = ['users', 'locations', 'payments', 'approvals'];
  route = 'buyer';
}
