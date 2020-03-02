import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { OrchestrationLog } from '@app-seller/reports/models/orchestration-log';
import { OrchestrationLogsService } from '@app-seller/reports/orchestration-logs.service';

@Component({
  selector: 'app-orchestration-logs-table',
  templateUrl: './orchestration-logs-table.component.html',
  styleUrls: ['./orchestration-logs-table.component.scss'],
})
export class OrchestrationLogsTableComponent extends ResourceCrudComponent<OrchestrationLog> {
  route = '/reports/orchestration-logs';

  constructor(
    private service: OrchestrationLogsService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, service, router, activatedRoute, ngZone);
  }

  filterConfig = {
    Filters: [
      {
        Display: 'Logs On/Before',
        Path: 'timeStamp',
        Type: 'DateFilter',
      },
      // TO-DO - UPDATE MIDDLEWARE TO ACCEPT BOTH A FROM AND TO DATE
      {
        Display: 'Record Type',
        Path: 'RecordType',
        Values: ['Catalog', 'PriceSchedule', 'Product', 'ProductFacet', 'Spec', 'SpecOption', 'SpecProductAssignment', 'User'],
        Type: 'Dropdown',
      },
      {
        Display: 'Action',
        Path: 'Action',
        Values: ['Create', 'Delete', 'Get', 'Ignore', 'Update'],
        Type: 'Dropdown',
      },
      {
        Display: 'Result',
        Path: 'Level',
        Values: ['Error', 'Success', 'Warn'],
        Type: 'Dropdown',
      },
    ],
  };
}
