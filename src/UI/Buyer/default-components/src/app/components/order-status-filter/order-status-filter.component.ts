import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OrderStatus, OrderFilters } from 'marketplace';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './order-status-filter.component.html',
  styleUrls: ['./order-status-filter.component.scss'],
})
export class OCMOrderStatusFilter extends OCMComponent implements OnInit {
  form: FormGroup;
  statuses = [
    OrderStatus.AllSubmitted,
    OrderStatus.Open,
    OrderStatus.AwaitingApproval,
    OrderStatus.Completed,
    OrderStatus.Declined,
    OrderStatus.Canceled,
  ];

  ngOnInit(): void {
    this.form = new FormGroup({
      status: new FormControl(OrderStatus.AllSubmitted),
    });
  }

  ngOnContextSet() {
    this.context.orderHistory.filters.onFiltersChange((filters: OrderFilters) => {
      this.form.setValue({ status: filters.status });
    });
  }

  selectStatus(): void {
    const status = this.form.get('status').value;
    this.context.orderHistory.filters.filterByStatus(status);
  }
}
