import { Component, Input, EventEmitter, Output } from '@angular/core';
import { ListOrder } from '@ordercloud/angular-sdk';
import { OrderListColumn } from '@app-buyer/order/models/order-list-column';
import { faCaretDown, faCaretUp } from '@fortawesome/free-solid-svg-icons';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';

@Component({
  selector: 'order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss'],
})
export class OrderListComponent {
  @Input() orders: ListOrder;
  @Input() columns: OrderListColumn[];
  @Input() sortBy: string;
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;
  @Output() updatedSort = new EventEmitter<string>();
  @Output() changedPage = new EventEmitter<number>();

  constructor(protected currentUser: CurrentUserService) {}

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
    return this.currentUser.favoriteOrderIDs.includes(orderID);
  }

  setIsFavorite(isFav: boolean, orderID: string) {
    this.currentUser.setIsFavoriteOrder(isFav, orderID);
  }
}
