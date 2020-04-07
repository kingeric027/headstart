import { Pipe, PipeTransform } from '@angular/core';
import { OrderStatus } from 'marketplace';

@Pipe({
  name: 'orderStatusDisplay',
})
export class OrderStatusDisplayPipe implements PipeTransform {
  OrderStatusMap = {
    [OrderStatus.AllSubmitted]: 'All Submitted',
    [OrderStatus.AwaitingApproval]: 'Awaiting Approval',
    [OrderStatus.ChangesRequested]: 'Changes Requested',
    [OrderStatus.Open]: 'Open',
    [OrderStatus.Completed]: 'Completed',
    [OrderStatus.Canceled]: 'Canceled',
  };

  transform(status: OrderStatus): string {
    if (!status) {
      return null;
    }
    const displayValue = this.OrderStatusMap[status];
    return displayValue;
  }
}
