import { Pipe, PipeTransform } from '@angular/core';
import { OrderStatus } from 'marketplace';

@Pipe({
  name: 'orderStatusDisplay',
})
export class OrderStatusDisplayPipe implements PipeTransform {
  OrderStatusMap = {
    [OrderStatus.Unsubmitted]: 'Unsubmitted',
    [OrderStatus.AwaitingApproval]: 'Awaiting Approval',
    [OrderStatus.Declined]: 'Declined',
    [OrderStatus.Open]: 'Open',
    [OrderStatus.Completed]: 'Completed',
    [OrderStatus.Canceled]: 'Canceled',
  };

  transform(status: OrderStatus) {
    if (!status) {
      return null;
    }
    return this.OrderStatusMap[status];
  }
}
