import { Component } from '@angular/core';
import { Order } from '@ordercloud/angular-sdk';
import { faCube, faTruck } from '@fortawesome/free-solid-svg-icons';
import { OCMComponent } from '../base-component';
import { OrderDetails } from 'marketplace';

@Component({
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OCMOrderDetails extends OCMComponent {
  order: Order;
  orderDetails: OrderDetails;
  approvalVersion: boolean;
  faCube = faCube;
  faTruck = faTruck;
  subView: 'details' | 'shipments' = 'details';

  async ngOnContextSet() {
    this.orderDetails = await this.context.orderHistory.getOrderDetails();
    this.order = this.orderDetails.order;
    const url = this.context.router.getActiveUrl();
    this.approvalVersion = url.includes('/approval');
  }

  isFavorite(orderID: string): boolean {
    return this.context.currentUser.favoriteOrderIDs.includes(orderID);
  }

  toggleFavorite(order: Order) {
    const newValue = !this.isFavorite(order.ID);
    this.context.currentUser.setIsFavoriteOrder(newValue, order.ID);
  }

  toShipments() {
    this.subView = 'shipments';
  }

  toDetails() {
    this.subView = 'details';
  }

  showShipments(): boolean {
    return this.subView === 'shipments';
  }

  showDetails(): boolean {
    return this.subView === 'details';
  }
}
