import { Component, OnInit } from '@angular/core';
import { find as _find } from 'lodash';
import { ShipmentWithItems, ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './order-shipments.component.html',
  styleUrls: ['./order-shipments.component.scss'],
})
export class OCMOrderShipments implements OnInit {
  selectedShipment: ShipmentWithItems;
  shipments: ShipmentWithItems[];

  constructor(private context: ShopperContextService) {}

  async ngOnInit() {
    this.shipments = await this.context.orderHistory.listShipments();
    this.selectedShipment = this.shipments[0];
  }

  selectShipment(shipment: ShipmentWithItems): void {
    this.selectedShipment = shipment;
  }
}

