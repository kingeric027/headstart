import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { Buyer } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerService } from '../buyer.service';

@Component({
  selector: 'buyer-table',
  templateUrl: './buyer-table.component.html',
  styleUrls: ['./buyer-table.component.scss'],
})
export class BuyerTableComponent extends ResourceCrudComponent<Buyer> {
  constructor(
    private buyerService: BuyerService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, buyerService, router, activatedRoute, ngZone);
  }

  route = 'buyer';
}
