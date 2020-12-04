import { Component, OnInit } from '@angular/core'
import { MarketplaceShipmentWithItems } from '@ordercloud/headstart-sdk'
import { ActivatedRoute } from '@angular/router'
import { OrderHistoryService } from 'src/app/services/order-history/order-history.service'
import { ShopperContextService } from 'src/app/services/shopper-context/shopper-context.service'

@Component({
  templateUrl: './order-shipments.component.html',
  styleUrls: ['./order-shipments.component.scss'],
})
export class OCMOrderShipments implements OnInit {
  selectedShipment: MarketplaceShipmentWithItems
  shipments: MarketplaceShipmentWithItems[]

  constructor(private context: ShopperContextService,
         private activatedRoute: ActivatedRoute,
        private orderHistory: OrderHistoryService) {
          this.orderHistory.activeOrderID = this.activatedRoute.snapshot.params.orderID
        }

  async ngOnInit(): Promise<void> {
    this.shipments = await this.context.orderHistory.listShipments()
    this.selectedShipment = this.shipments[0]
  }

  selectShipment(shipment: MarketplaceShipmentWithItems): void {
    this.selectedShipment = shipment
  }
}
