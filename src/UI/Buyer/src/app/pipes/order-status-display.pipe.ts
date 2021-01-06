import { Pipe, PipeTransform } from '@angular/core'
import { LineItemStatus } from '../models/line-item.types'
import { ClaimStatus, MarketplaceOrderStatus } from '../models/order.types'
import { ShippingStatus } from '../models/shipping.types'

@Pipe({
  name: 'orderStatusDisplay',
})
export class OrderStatusDisplayPipe implements PipeTransform {
  OrderStatusMap = {
    [MarketplaceOrderStatus.AllSubmitted]: 'All Submitted',
    [MarketplaceOrderStatus.AwaitingApproval]: 'Awaiting Approval',
    [MarketplaceOrderStatus.ChangesRequested]: 'Changes Requested',
    [MarketplaceOrderStatus.Open]: 'Open',
    [MarketplaceOrderStatus.Completed]: 'Completed',
    [MarketplaceOrderStatus.Canceled]: 'Canceled',
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

  transform(status: MarketplaceOrderStatus): string {
    if (!status) {
      return null
    }
    const displayValue = this.OrderStatusMap[status]
    return displayValue
  }
}
