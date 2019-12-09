import { Component, Input, EventEmitter, Output } from '@angular/core';
import { ListOrder } from '@ordercloud/angular-sdk';
import { faCaretDown, faCaretUp } from '@fortawesome/free-solid-svg-icons';
import { OrderListColumn } from '../../../models/order-list-column';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss'],
})
export class OCMOrderList {
  @Input() orders: ListOrder;
  @Input() columns: OrderListColumn[];
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;

  // todo - should this be in some kinda service?
  @Input() sortBy: string;
  @Output() updatedSort = new EventEmitter<string>();
  @Output() changedPage = new EventEmitter<number>();

  constructor(private context: ShopperContextService) {}

  updateSort(selectedSortBy) {
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

  isFavorite(orderID: string) {
    return this.context.currentUser.favoriteOrderIDs.includes(orderID);
  }

  setIsFavorite(isFav: boolean, orderID: string) {
    this.context.currentUser.setIsFavoriteOrder(isFav, orderID);
  }

  toOrderDetails(orderID: string) {
    this.context.router.toMyOrderDetails(orderID);
  }
}
