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
        Display: 'FILTERS.LOGS_ON_BEFORE',
        Path: 'timeStamp',
        Type: 'DateFilter',
      },
      // TO-DO - UPDATE MIDDLEWARE TO ACCEPT BOTH A FROM AND TO DATE
      {
        Display: 'FILTERS.RECORD_TYPE',
        Path: 'RecordType',
        Items: [
          {Text: 'FILTER_OPTIONS.CATALOG', Value: 'Catalog'},
          {Text: 'FILTER_OPTIONS.PRICE_SCHEDULE', Value: 'PriceSchedule'},
          {Text: 'FILTER_OPTIONS.PRODUCT', Value: 'Product'},
          {Text: 'FILTER_OPTIONS.PRODUCT_FACET', Value: 'ProductFacet'},
          {Text: 'FILTER_OPTIONS.SPEC', Value: 'Spec'},
          {Text: 'FILTER_OPTIONS.SPEC_OPTION', Value: 'SpecOption'},
          {Text: 'FILTER_OPTIONS.SPEC_PRODUCT_ASSIGNMENT', Value: 'Spec'},
          {Text: 'FILTER_OPTIONS.USER', Value: 'User'}
        ],
        Type: 'Dropdown',
      },
      {
        Display: 'FILTERS.ACTION',
        Path: 'Action',
        Items: [
          {Text: 'FILTER_OPTIONS.CREATE', Value: 'Create'},
          {Text: 'FILTER_OPTIONS.DELETE', Value: 'Delete'},
          {Text: 'FILTER_OPTIONS.GET', Value: 'Get'},
          {Text: 'FILTER_OPTIONS.IGNORE', Value: 'Ignore'},
          {Text: 'FILTER_OPTIONS.UPDATE', Value: 'Update'},],
        Type: 'Dropdown',
      },
      {
        Display: 'FILTERS.RESULT',
        Path: 'Level',
        Items: [
          {Text: 'FILTER_OPTIONS.ERROR', Value: 'Error'},
          {Text: 'FILTER_OPTIONS.SUCCESS', Value: 'Success'},
          {Text: 'FILTER_OPTIONS.WARN', Value: 'Warn'}],
        Type: 'Dropdown',
      },
    ],
  };
}
