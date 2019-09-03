import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { OcOrderService, OcLineItemService } from '@ordercloud/angular-sdk';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { listAll } from '@app-buyer/shared/functions/listAll';

@Injectable({
  providedIn: 'root',
})
export class OrderResolve implements Resolve<any> {
  constructor(private ocOrderService: OcOrderService, private ocLineItemService: OcLineItemService) {}

  resolve(route: ActivatedRouteSnapshot) {
    const orderID = route.paramMap.get('orderID');
    return forkJoin([this.ocOrderService.Get('outgoing', orderID), listAll(this.ocLineItemService, 'outgoing', orderID)]).pipe(
      map((results) => ({ order: results[0], lineItems: results[1] }))
    );
  }
}
