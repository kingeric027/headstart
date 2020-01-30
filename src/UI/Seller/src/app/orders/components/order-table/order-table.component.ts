import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Order } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { OrderService } from '@app-seller/shared/services/order/order.service';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
import { SELLER } from '@app-seller/shared/models/ordercloud-user.types';

@Component({
  selector: 'app-order-table',
  templateUrl: './order-table.component.html',
  styleUrls: ['./order-table.component.scss'],
})
export class OrderTableComponent extends ResourceCrudComponent<Order> {
  shouldShowOrderToggle = false;
  activeOrderDirectionButton: string;
  constructor(
    private orderService: OrderService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    ngZone: NgZone,
    private appAuthService: AppAuthService
  ) {
    super(changeDetectorRef, orderService, router, activatedroute, ngZone);
    this.shouldShowOrderToggle = this.appAuthService.getOrdercloudUserType() === SELLER;
    activatedroute.queryParams.subscribe(params => {
      if (this.router.url.startsWith('/orders')) {
        this.readFromUrlQueryParams(params);
        console.log(this.orderService);
      }
    });
  }
  setOrderDirection(direction: string) {
    this.orderService.setOrderDirection(direction);
  }

  private readFromUrlQueryParams(params: Params): void {
    const { OrderDirection } = params;
    this.activeOrderDirectionButton = OrderDirection;
  }
}
