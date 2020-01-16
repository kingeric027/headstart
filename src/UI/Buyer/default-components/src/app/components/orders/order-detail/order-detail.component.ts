import { Component, OnInit } from '@angular/core';
import { faCube, faTruck } from '@fortawesome/free-solid-svg-icons';
import { OrderDetails, ShopperContextService, MarketplaceOrder } from 'marketplace';

@Component({
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OCMOrderDetails implements OnInit {
  order: MarketplaceOrder;
  orderDetails: OrderDetails;
  approvalVersion: boolean;
  faCube = faCube;
  faTruck = faTruck;
  subView: 'details' | 'shipments' = 'details';

  constructor(private context: ShopperContextService) {}

  async ngOnInit() {
    this.orderDetails = await this.context.orderHistory.getOrderDetails();
    this.order = this.orderDetails.order;
    const url = this.context.router.getActiveUrl();
    this.approvalVersion = url.includes('/approval');
  }

  isFavorite(orderID: string): boolean {
    return this.context.currentUser.favoriteOrderIDs.includes(orderID);
  }

  toggleFavorite(order: MarketplaceOrder) {
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
