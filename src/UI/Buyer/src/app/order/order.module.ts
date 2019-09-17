import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';
import { DatePipe } from '@angular/common';

import { OrderRoutingModule, MyOrdersComponent, OrdersToApproveComponent } from 'src/app/order/order-routing.module';
import { OrderHistoryComponent } from 'src/app/order/containers/order-history/order-history.component';
import { StatusFilterComponent } from 'src/app/order/components/status-filter/status-filter.component';
import { DateFilterComponent } from 'src/app/order/components/date-filter/date-filter.component';
import { OrderListComponent } from 'src/app/order/components/order-list/order-list.component';
import { OrderDetailsComponent } from 'src/app/order/containers/order-detail/order-detail.component';
import { StatusIconComponent } from 'src/app/order/components/status-icon/status-icon.component';
import { OrderComponent } from 'src/app/order/containers/order/order.component';
import { OrderShipmentsComponent } from 'src/app/order/containers/order-shipments/order-shipments.component';
import { OrderReorderComponent } from 'src/app/order/containers/order-reorder/order-reorder.component';
import { OrderApprovalComponent } from 'src/app/order/containers/order-approval/order-approval.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, OrderRoutingModule],
  declarations: [
    OrderHistoryComponent,
    StatusFilterComponent,
    DateFilterComponent,
    OrderListComponent,
    OrderDetailsComponent,
    StatusIconComponent,
    OrderComponent,
    OrderShipmentsComponent,
    OrderReorderComponent,
    MyOrdersComponent,
    OrdersToApproveComponent,
    OrderApprovalComponent,
  ],
  providers: [
    DatePipe, // allows us to use in class as injectable (date filter component)
  ],
})
export class OrderModule {}
