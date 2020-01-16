import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrderTableComponent } from './components/order-list/order-table.component';
import { OrderDetailsComponent } from './components/order-details/order-details.component';
import { OrderShipmentsComponent } from './components/order-shipments/order-shipments.component';

@NgModule({
  imports: [SharedModule, OrdersRoutingModule],
  declarations: [OrderTableComponent],
})
export class OrdersModule {}
