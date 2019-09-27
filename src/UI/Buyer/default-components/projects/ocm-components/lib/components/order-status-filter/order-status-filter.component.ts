import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { OrderStatus } from 'src/app/order/models/order-status.model';
import { FormGroup, FormControl } from '@angular/forms';
import { OCMComponent } from '../../shopper-context';

@Component({
  templateUrl: './order-status-filter.component.html',
  styleUrls: ['./order-status-filter.component.scss'],
})
export class OCMOrderStatusFilter extends OCMComponent implements OnInit {
  form: FormGroup;
  statuses: OrderStatus[];
  @Output() selectedStatus = new EventEmitter<OrderStatus>();

  ngOnInit(): void {
    this.form = new FormGroup({
      status: new FormControl(`!${OrderStatus.Unsubmitted}`),
    });
    this.statuses = [OrderStatus.Open, OrderStatus.AwaitingApproval, OrderStatus.Completed, OrderStatus.Declined, OrderStatus.Canceled];
  }

  selectStatus(): void {
    const status = this.form.get('status').value;
    this.selectedStatus.emit(status);
  }
}
