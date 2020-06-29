import { Pipe, PipeTransform } from '@angular/core';
import { OrderStatus, ShippingStatus, LineItemStatus, ClaimStatus } from 'marketplace';

@Pipe({
  name: 'orderStatusDisplay',
})
export class OrderStatusDisplayPipe implements PipeTransform {
  OrderStatusMap = {
    [OrderStatus.AllSubmitted]: 'All Submitted',
    [OrderStatus.AwaitingApproval]: 'Awaiting Approval',
    [OrderStatus.ChangesRequested]: 'Changes Requested',
    [OrderStatus.Open]: 'Open',
    [OrderStatus.Complete]: 'Complete',
    [OrderStatus.Canceled]: 'Canceled',
    [ShippingStatus.Shipped]: 'Shipped',
    [ShippingStatus.Backordered]: 'Backordered',
    [ShippingStatus.Processing]: 'Processing',
    [ShippingStatus.PartiallyShipped]: 'Partially Shipped',
    [LineItemStatus.ReturnRequested]: 'Return Requested',
    [LineItemStatus.Returned]: 'Returned',
    [LineItemStatus.Submitted]: 'Submitted',
    [ClaimStatus.Pending]: 'Pending',
    [ClaimStatus.NoClaim]: 'No Claim'
  };

  transform(status: OrderStatus): string {
    if (!status) {
      return null;
    }
    const displayValue = this.OrderStatusMap[status];
    return displayValue;
  }
}
