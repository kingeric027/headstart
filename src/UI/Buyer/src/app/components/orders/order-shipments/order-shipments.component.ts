import { Component, Input } from '@angular/core'
import { MarketplaceShipmentWithItems } from '@ordercloud/headstart-sdk'
import { ShopperContextService } from 'src/app/services/shopper-context/shopper-context.service'

@Component({
  templateUrl: './order-shipments.component.html',
  styleUrls: ['./order-shipments.component.scss'],
})
export class OCMOrderShipments {
  @Input() set orderID(_orderID: string) {
    if (_orderID) {
      void this.init(_orderID)
    }
  }
  selectedShipment: MarketplaceShipmentWithItems
  shipments: MarketplaceShipmentWithItems[]

  constructor(private context: ShopperContextService) {}

  async init(orderID: string): Promise<void> {
    this.shipments = await this.context.orderHistory.listShipments(orderID)
    this.selectedShipment = this.shipments[0]
  }

  selectShipment(shipment: MarketplaceShipmentWithItems): void {
    this.selectedShipment = shipment
  }
}
