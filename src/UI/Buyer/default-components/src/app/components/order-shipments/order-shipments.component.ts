import { Component } from '@angular/core';
import { find as _find } from 'lodash';
import { OCMComponent } from '../base-component';
import { MyShipment } from 'marketplace';

@Component({
  templateUrl: './order-shipments.component.html',
  styleUrls: ['./order-shipments.component.scss'],
})
export class OCMOrderShipments extends OCMComponent {
  selectedShipment: MyShipment;
  shipments: MyShipment[];

  async ngOnContextSet() {
    this.shipments = await this.context.orderHistory.listShipments();
    this.selectedShipment = this.shipments[0];
  }

  selectShipment(shipment: MyShipment): void {
    this.selectedShipment = shipment;
  }
}

