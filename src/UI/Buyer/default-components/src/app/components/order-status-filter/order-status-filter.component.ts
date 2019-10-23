import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OrderStatus } from 'marketplace';

@Component({
  templateUrl: './order-status-filter.component.html',
  styleUrls: ['./order-status-filter.component.scss'],
})
export class OCMOrderStatusFilter implements OnInit {
  form: FormGroup;
  statuses: OrderStatus[];
  @Output() selectedStatus = new EventEmitter<OrderStatus>();

  ngOnInit(): void {
    this.form = new FormGroup({
      status: new FormControl(OrderStatus.AllSubmitted),
    });
    this.statuses = [OrderStatus.AllSubmitted, OrderStatus.Open, OrderStatus.AwaitingApproval, OrderStatus.Completed, OrderStatus.Declined, OrderStatus.Canceled];
  }

  selectStatus(): void {
    const status = this.form.get('status').value;
    this.selectedStatus.emit(status);
  }
}
