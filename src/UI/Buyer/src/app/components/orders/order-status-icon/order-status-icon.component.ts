import { Component, Input } from '@angular/core'
import { faCircle, faClock, faBan } from '@fortawesome/free-solid-svg-icons'
import { ClaimStatus, LineItemStatus, OrderStatus, ShippingStatus } from 'src/app/shopper-context'

@Component({
  templateUrl: './order-status-icon.component.html',
  styleUrls: ['./order-status-icon.component.scss'],
})
export class OCMOrderStatusIcon {
  @Input() status: OrderStatus
  faCircle = faCircle
  faClock = faClock
  faBan = faBan
  statusIconMapping = {
    [OrderStatus.Completed]: this.faCircle,
    [OrderStatus.AwaitingApproval]: this.faClock,
    [OrderStatus.Open]: this.faCircle,
    // [OrderStatus.Declined]: this.faCircle,
    [OrderStatus.Canceled]: this.faBan,
    [ClaimStatus.Pending]: this.faClock,
    [ClaimStatus.NoClaim]: this.faCircle,
    [ShippingStatus.PartiallyShipped]: this.faCircle,
    [ShippingStatus.Processing]: this.faClock,
    [ShippingStatus.Shipped]: this.faCircle,
    [LineItemStatus.Backordered]: this.faClock,
    [LineItemStatus.Complete]: this.faCircle,
    [LineItemStatus.Returned]: this.faCircle,
    [LineItemStatus.Submitted]: this.faCircle,
    [LineItemStatus.ReturnRequested]: this.faClock,
    [LineItemStatus.CancelRequested]: this.faClock,
  }
}