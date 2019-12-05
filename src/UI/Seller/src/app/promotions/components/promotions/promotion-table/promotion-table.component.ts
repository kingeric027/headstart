import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { Promotion } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { PromotionService } from '@app-seller/shared/services/promotion/promotion.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-promotion-table',
  templateUrl: './promotion-table.component.html',
  styleUrls: ['./promotion-table.component.scss'],
})
export class PromotionTableComponent extends ResourceCrudComponent<Promotion> {
  constructor(
    private promotionService: PromotionService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, promotionService, router, activatedRoute, ngZone);
  }
}
