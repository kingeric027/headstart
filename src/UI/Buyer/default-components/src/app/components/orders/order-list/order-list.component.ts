import { Component, Input, EventEmitter, Output } from '@angular/core';
import { faCaretDown, faCaretUp } from '@fortawesome/free-solid-svg-icons';
import { OrderListColumn } from '../../../models/order-list-column';
import { ShopperContextService, OrderViewContext } from 'marketplace';
import { ListPage } from 'ordercloud-javascript-sdk';
import { isQuoteOrder } from '../../../services/orderType.helper';
import { MarketplaceOrder } from 'marketplace-javascript-sdk';

@Component({
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss'],
})
export class OCMOrderList {
  @Input() orders: ListPage<MarketplaceOrder>;
  @Input() columns: OrderListColumn[];
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;
  isQuoteOrder = isQuoteOrder;
  // todo - should this be in some kinda service?
  @Input() sortBy: string;
  @Output() updatedSort = new EventEmitter<string>();
  @Output() changedPage = new EventEmitter<number>();

  constructor(private context: ShopperContextService) {}

  updateSort(selectedSortBy: string): void {
    let sortBy;
    switch (this.sortBy) {
      case selectedSortBy:
        sortBy = `!${selectedSortBy}`;
        break;
      case `!${selectedSortBy}`:
        // setting to undefined so sdk ignores parameter
        sortBy = undefined;
        break;
      default:
        sortBy = selectedSortBy;
    }
    this.updatedSort.emit(sortBy);
  }

  changePage(page: number): void {
    this.changedPage.emit(page);
  }

  isFavorite(orderID: string): boolean {
    return this.context.currentUser.get().FavoriteOrderIDs.includes(orderID);
  }

  setIsFavorite(isFav: boolean, orderID: string): void {
    this.context.currentUser.setIsFavoriteOrder(isFav, orderID);
  }

  toOrderDetails(orderID: string): void {
    const isOrderToApprove = this.context.router.getOrderViewContext() === OrderViewContext.Approve;
    if (isOrderToApprove) {
      this.context.router.toOrderToAppoveDetails(orderID);
    } else {
      this.context.router.toMyOrderDetails(orderID);
    }
  }
}
