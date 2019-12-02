import { Component, OnInit, EventEmitter, Output, OnDestroy } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OrderStatus, OrderFilters, ShopperContextService } from 'marketplace';
import { takeWhile } from 'rxjs/operators';

@Component({
  templateUrl: './order-status-filter.component.html',
  styleUrls: ['./order-status-filter.component.scss'],
})
export class OCMOrderStatusFilter implements OnInit, OnDestroy {
  alive = true;
  form: FormGroup;
  statuses = [
    OrderStatus.AllSubmitted,
    OrderStatus.Open,
    OrderStatus.AwaitingApproval,
    OrderStatus.Completed,
    OrderStatus.Declined,
    OrderStatus.Canceled,
  ];

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.form = new FormGroup({
      status: new FormControl(OrderStatus.AllSubmitted),
    });
    this.context.orderHistory.filters.activeFiltersSubject.pipe(takeWhile(() => this.alive)).subscribe((filters: OrderFilters) => {
      this.form.setValue({ status: filters.status || '' });
    });
  }

  selectStatus(): void {
    const status = this.form.get('status').value;
    this.context.orderHistory.filters.filterByStatus(status);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
