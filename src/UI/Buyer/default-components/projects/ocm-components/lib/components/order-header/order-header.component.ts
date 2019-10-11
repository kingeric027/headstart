import { Component, Input } from '@angular/core';
import { faCube, faTruck } from '@fortawesome/free-solid-svg-icons';
import { Order } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './order-header.component.html',
  styleUrls: ['./order-header.component.scss'],
})
export class OCMOrderHeader extends OCMComponent {
  @Input() order: Order;
  approvalVersion: boolean;
  faCube = faCube;
  faTruck = faTruck;

  ngOnContextSet() {
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
}
