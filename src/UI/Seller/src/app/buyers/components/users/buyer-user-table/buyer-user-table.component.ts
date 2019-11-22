import { Component, ChangeDetectorRef } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { User } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerUserService } from '@app-seller/shared/services/buyer/buyer-user.service';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';

@Component({
  selector: 'app-buyer-user-table',
  templateUrl: './buyer-user-table.component.html',
  styleUrls: ['./buyer-user-table.component.scss'],
})
export class BuyerUserTableComponent extends ResourceCrudComponent<User> {
  constructor(
    private buyerUserService: BuyerUserService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private buyerService: BuyerService
  ) {
    super(changeDetectorRef, buyerUserService, router, activatedroute);
  }
}
