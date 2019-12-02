import { Component, Input } from '@angular/core';
import { faCircle, faClock, faBan } from '@fortawesome/free-solid-svg-icons';
import { OCMComponent } from '../../base-component';
import { OrderStatus } from 'marketplace';

@Component({
  templateUrl: './order-status-icon.component.html',
  styleUrls: ['./order-status-icon.component.scss'],
})
export class OCMOrderStatusIcon {
  @Input() status: OrderStatus;
  faCircle = faCircle;
  faClock = faClock;
  faBan = faBan;
  statusIconMapping = {
    [OrderStatus.Completed]: this.faCircle,
    [OrderStatus.AwaitingApproval]: this.faClock,
    [OrderStatus.Open]: this.faCircle,
    [OrderStatus.Declined]: this.faCircle,
    [OrderStatus.Canceled]: this.faBan,
  };
}
