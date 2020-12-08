import { Pipe, PipeTransform } from '@angular/core'
import { ClaimStatus, LineItemStatus, OrderStatus, ShippingStatus } from '../shopper-context'

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
    [ShippingStatus.Shipped]: 'Shipped',
    [ShippingStatus.Backordered]: 'Backordered',
    [ShippingStatus.Processing]: 'Processing',
    [ShippingStatus.PartiallyShipped]: 'Partially Shipped',
    [LineItemStatus.ReturnRequested]: 'Return Requested',
    [LineItemStatus.CancelRequested]: 'Cancel Requested',
    [LineItemStatus.Returned]: 'Returned',
    [LineItemStatus.Complete]: 'Complete',
    [LineItemStatus.Submitted]: 'Submitted',
    [ClaimStatus.Pending]: 'Pending',
    [ClaimStatus.NoClaim]: 'No Claim',
  }

  transform(status: OrderStatus): string {
    if (!status) {
      return null
    }
    const displayValue = this.OrderStatusMap[status]
    return displayValue
  }
}