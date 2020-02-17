import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { OrchestrationLog } from '@app-seller/reports/models/orchestration-log';
import { Buyer } from '@ordercloud/angular-sdk';
import { BuyerService } from '@app-seller/buyers/components/buyers/buyer.service';

@Component({
  selector: 'app-orchestration-logs-table',
  templateUrl: './orchestration-logs-table.component.html',
  styleUrls: ['./orchestration-logs-table.component.scss'],
})
export class OrchestrationLogsTableComponent extends ResourceCrudComponent<Buyer> {
  route = '/reports/orchestration-logs';

  constructor(
    private buyerService: BuyerService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, buyerService, router, activatedRoute, ngZone);
  }
}
