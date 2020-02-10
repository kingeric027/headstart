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
  isListPage: boolean;
  shouldShowOrderToggle: boolean = false;
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
      }
    });
    activatedroute.params.subscribe(params => {
      this.isListPage = !Boolean(params.orderID);
    })
  }
  setOrderDirection(direction: string) {
    if (this.isListPage) {
      this.orderService.setOrderDirection(direction);
    } else {
      this.router.navigate(['/orders'], { queryParams: { OrderDirection: direction } });
    }
  }

  private readFromUrlQueryParams(params: Params): void {
    const { OrderDirection } = params;
    this.activeOrderDirectionButton = OrderDirection;
  }
  filterConfig = {
    Filters: [
      {
        Display: 'Status',
        Path: 'Status',
        Values: ['Open', 'AwaitingApproval', 'Completed', 'Declined', 'Canceled'],
        Type: 'Dropdown'
      },
      {
        Display: 'From Date',
        Path: 'from',
        Type: 'DateFilter'
      },
      {
        Display: 'To Date',
        Path: 'to',
        Type: 'DateFilter'
      },
    ],
  };
}
