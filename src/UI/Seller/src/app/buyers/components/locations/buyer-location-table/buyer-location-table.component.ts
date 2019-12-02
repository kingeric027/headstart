import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { BuyerAddress } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { BuyerLocationService } from '@app-seller/shared/services/buyer/buyer-location.service';

@Component({
  selector: 'app-buyer-location-table',
  templateUrl: './buyer-location-table.component.html',
  styleUrls: ['./buyer-location-table.component.scss'],
})
export class BuyerLocationTableComponent extends ResourceCrudComponent<BuyerAddress> {
  constructor(
    private buyerLocationService: BuyerLocationService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private buyerService: BuyerService,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, buyerLocationService, router, activatedroute, ngZone);
  }
}
