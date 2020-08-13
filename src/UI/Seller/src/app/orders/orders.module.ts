import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrderTableComponent } from './components/order-table/order-table.component';
import { OrderDetailsComponent } from './components/order-details/order-details.component';
import { OrderShipmentsComponent } from './components/order-shipments/order-shipments.component';
import { LineItemTableComponent } from './components/line-item-table/line-item-table.component';

@NgModule({
  imports: [SharedModule, OrdersRoutingModule],
  declarations: [OrderTableComponent, OrderDetailsComponent, OrderShipmentsComponent, LineItemTableComponent],
})
export class OrdersModule {}
