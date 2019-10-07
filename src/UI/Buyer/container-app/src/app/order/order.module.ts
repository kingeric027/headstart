import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';

import { OrderRoutingModule, MyOrdersComponent, OrdersToApproveComponent } from 'src/app/order/order-routing.module';
import { OrderHistoryComponent } from 'src/app/order/containers/order-history/order-history.component';
import { OrderDetailsComponent } from 'src/app/order/containers/order-detail/order-detail.component';
import { OrderComponent } from 'src/app/order/containers/order/order.component';
import { OrderShipmentsComponent } from 'src/app/order/containers/order-shipments/order-shipments.component';
import { OrderReorderComponent } from 'src/app/order/containers/order-reorder/order-reorder.component';
import { OrderApprovalComponent } from 'src/app/order/containers/order-approval/order-approval.component';
import { OrderDetailWrapperComponent } from './components/order-detail-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, OrderRoutingModule],
  declarations: [
    OrderHistoryComponent,
    OrderDetailsComponent,
    OrderComponent,
    OrderShipmentsComponent,
    OrderReorderComponent,
    MyOrdersComponent,
    OrdersToApproveComponent,
    OrderApprovalComponent,
    OrderDetailWrapperComponent,
  ],
})
export class OrderModule {}
