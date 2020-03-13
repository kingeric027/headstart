import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcBuyerService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { OrchestrationLog, ListPage, MarketplaceSDK } from 'marketplace-javascript-sdk';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class OrchestrationLogsService extends ResourceCrudService<OrchestrationLog> {
  primaryResourceLevel = 'logs';

  constructor(router: Router, activatedRoute: ActivatedRoute, service: OcBuyerService) {
    super(router, activatedRoute, service, '/reports/logs', 'Logs');
  }

  async list(args: any[]): Promise<ListPage<OrchestrationLog>> {
    const listArgs: ListArgs = args[0];
    listArgs.sortBy = listArgs.sortBy || '!timeStamp';
    return await MarketplaceSDK.OrchestrationLogs.List(listArgs);
  }
}
