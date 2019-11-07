import { Component, AfterViewInit, Input } from '@angular/core';
import { ListOrder } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../base-component';
import { OrderStatus, OrderFilters } from 'marketplace';
import { takeWhile } from 'rxjs/operators';

@Component({
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.scss'],
})
export class OCMOrderHistory extends OCMComponent implements AfterViewInit {
  columns: string[] = ['ID', 'Status', 'DateSubmitted', 'Total'];
  @Input() orders: ListOrder;
  @Input() approvalVersion: boolean;
  showOnlyFavorites = false;
  sortBy: string;
  searchTerm: string;

  ngOnContextSet() {
    this.context.orderHistory.filters.activeFiltersSubject.pipe(takeWhile(() => this.alive)).subscribe(this.handleFiltersChange);
  }

  handleFiltersChange = (filters: OrderFilters) => {
    this.sortBy = filters.sortBy;
    this.showOnlyFavorites = filters.showOnlyFavorites;
    this.searchTerm = filters.search;
  }

  async ngAfterViewInit() {
    if (!this.approvalVersion) {
      this.columns.push('Favorite');
    }
  }

  sortOrders(sortBy: string): void {
    this.context.orderHistory.filters.sortBy(sortBy);
  }

  changePage(page: number): void {
    this.context.orderHistory.filters.toPage(page);
  }

  filterBySearch(search: string): void {
    this.context.orderHistory.filters.searchBy(search);
  }

  filterByStatus(status: OrderStatus): void {
    this.context.orderHistory.filters.filterByStatus(status);
  }

  filterByFavorite(showOnlyFavorites: boolean): void {
    this.context.orderHistory.filters.filterByFavorites(showOnlyFavorites);
  }

}
