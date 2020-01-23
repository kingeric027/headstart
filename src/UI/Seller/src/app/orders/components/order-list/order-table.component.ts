import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Order, OcOrderService } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { SellerUserService } from '@app-seller/shared/services/seller-user/seller-user.service';
import { FormControl, FormGroup } from '@angular/forms';
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
  buttonType: string;
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
  }
  setOrderDirection(direction: string) {
    this.orderService.addFilters({ OrderDirection: direction });
    this.buttonType = direction;
  }
}
