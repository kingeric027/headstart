import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'marketplace';
import { MarketplaceShipmentWithItems } from '@ordercloud/headstart-sdk';

@Component({
  templateUrl: './order-shipments.component.html',
  styleUrls: ['./order-shipments.component.scss'],
})
export class OCMOrderShipments implements OnInit {
  selectedShipment: MarketplaceShipmentWithItems;
  shipments: MarketplaceShipmentWithItems[];

  constructor(private context: ShopperContextService) {}

  async ngOnInit(): Promise<void> {
    this.shipments = await this.context.orderHistory.listShipments();
    this.selectedShipment = this.shipments[0];
  }

  selectShipment(shipment: MarketplaceShipmentWithItems): void {
    this.selectedShipment = shipment;
  }
}
