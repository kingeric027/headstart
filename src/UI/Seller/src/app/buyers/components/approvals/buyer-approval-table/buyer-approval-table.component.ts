import { Component, ChangeDetectorRef } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { ApprovalRule } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { BuyerApprovalService } from '@app-seller/shared/services/buyer/buyer-approval.service';

@Component({
  selector: 'app-buyer-approval-table',
  templateUrl: './buyer-approval-table.component.html',
  styleUrls: ['./buyer-approval-table.component.scss'],
})
export class BuyerApprovalTableComponent extends ResourceCrudComponent<ApprovalRule> {
  constructor(
    private buyerApprovalService: BuyerApprovalService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private buyerService: BuyerService
  ) {
    super(changeDetectorRef, buyerApprovalService, router, activatedroute);
  }
}
