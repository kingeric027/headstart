import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcBuyerService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { OrchestrationLog } from './models/orchestration-log';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class OrchestrationLogsService extends ResourceCrudService<OrchestrationLog> {
  primaryResourceLevel = 'orchestrationLogs';

  constructor(router: Router, activatedRoute: ActivatedRoute, service: OcBuyerService) {
    super(router, activatedRoute, service, '/orchestrationLogs', 'Logs');
  }
}
