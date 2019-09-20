import { Component, Input, EventEmitter, Output } from '@angular/core';
import { ListOrder } from '@ordercloud/angular-sdk';
import { OrderListColumn } from 'src/app/order/models/order-list-column';
import { faCaretDown, faCaretUp } from '@fortawesome/free-solid-svg-icons';
import { OCMComponent } from 'src/app/ocm-default-components/shopper-context';

@Component({
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss'],
})
export class OCMOrderList extends OCMComponent {
  @Input() orders: ListOrder;
  @Input() columns: OrderListColumn[];
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;

  // todo - should this be in some kinda service?
  @Input() sortBy: string;
  @Output() updatedSort = new EventEmitter<string>();
  @Output() changedPage = new EventEmitter<number>();

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
    this.context.routeActions.toOrderDetails(orderID);
  }
}
