import { Component } from '@angular/core';
import { OrderDetailsComponent } from 'src/app/order/containers/order-detail/order-detail.component';

@Component({
  selector: 'checkout-order-confirmation',
  templateUrl: './order-confirmation.component.html',
  styleUrls: ['./order-confirmation.component.scss'],
})
export class OrderConfirmationComponent extends OrderDetailsComponent {}
