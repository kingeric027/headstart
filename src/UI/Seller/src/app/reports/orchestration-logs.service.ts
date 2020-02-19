import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcBuyerService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { OrchestrationLog } from './models/orchestration-log';
import { ListPage } from '@app-seller/shared/services/middleware-api/listPage.interface';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class OrchestrationLogsService extends ResourceCrudService<OrchestrationLog> {
  primaryResourceLevel = 'logs';

  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    service: OcBuyerService,
    private middleware: MiddlewareAPIService
  ) {
    super(router, activatedRoute, service, '/reports/logs', 'Logs');
  }

  async list(args: any[]): Promise<ListPage<OrchestrationLog>> {
    if (!this.middleware) return; // TODO - why is this undefined?
    // TODO - pass in args
    return await this.middleware.listOrchestrationLogs(args[0]);
  }
}
