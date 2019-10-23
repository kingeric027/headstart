import { Component, AfterViewInit, Input } from '@angular/core';
import { ListOrder } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../base-component';
import { OrderStatus } from 'marketplace';

@Component({
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.scss'],
})
export class OCMOrderHistory extends OCMComponent implements AfterViewInit {
  columns: string[] = ['ID', 'Status', 'DateSubmitted', 'Total'];
  @Input() orders: ListOrder;
  hasFavoriteOrdersFilter = false;
  sortBy: string;
  @Input() approvalVersion: boolean;

  ngOnContextSet() {}

  async ngAfterViewInit() {
    if (!this.approvalVersion) {
      this.columns.push('Favorite');
    }
  }

  sortOrders(sortBy: string): void {

  }

  changePage(page: number): void {

  }

  filterBySearch(search: string): void {

  }

  filterByStatus(status: OrderStatus): void {

  }

  filterByDate(datesubmitted: string[]): void {

  }

  filterByFavorite(favoriteOrders: boolean): void {

  }

}
