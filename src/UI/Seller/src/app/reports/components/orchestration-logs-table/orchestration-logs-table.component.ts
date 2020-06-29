import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { OrchestrationLogsService } from '@app-seller/reports/orchestration-logs.service';
import { OrchestrationLog } from 'marketplace-javascript-sdk';

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
        Items: [
          {Text: 'Catalog', Value: 'Catalog'},
          {Text: 'PriceSchedule', Value: 'PriceSchedule'},
          {Text: 'Product', Value: 'Product'},
          {Text: 'ProductFacet', Value: 'ProductFacet'},
          {Text: 'Spec', Value: 'Spec'},
          {Text: 'SpecOption', Value: 'SpecOption'},
          {Text: 'SpecProductAssignment', Value: 'Spec'},
          {Text: 'User', Value: 'User'}
        ],
        Type: 'Dropdown',
      },
      {
        Display: 'Action',
        Path: 'Action',
        Items: [
          {Text: 'Create', Value: 'Create'},
          {Text: 'Delete', Value: 'Delete'},
          {Text: 'Get', Value: 'Get'},
          {Text: 'Ignore', Value: 'Ignore'},
          {Text: 'Update', Value: 'Update'},],
        Type: 'Dropdown',
      },
      {
        Display: 'Result',
        Path: 'Level',
        Items: [
          {Text: 'Error', Value: 'Error'},
          {Text: 'Success', Value: 'Success'},
          {Text: 'Warn', Value: 'Warn'}],
        Type: 'Dropdown',
      },
    ],
  };
}
