import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { Promotion } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { PromotionService } from '@app-seller/shared/services/promotion/promotion.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-promotion-list',
  templateUrl: './promotion-list.component.html',
  styleUrls: ['./promotion-list.component.scss'],
})
export class PromotionListComponent extends ResourceCrudComponent<Promotion> {
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
