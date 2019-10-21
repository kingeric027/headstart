import { Component, OnInit } from '@angular/core';
import { ListShipment, Shipment, ListShipmentItem, ListLineItem } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { find as _find } from 'lodash';
import { ShopperContextService } from '../../../services/shopper-context/shopper-context.service';

@Component({
  selector: 'order-shipments',
  templateUrl: './order-shipments.component.html',
  styleUrls: ['./order-shipments.component.scss'],
})
export class OrderShipmentsComponent implements OnInit {
  selectedShipment: Shipment;
  shipments: ListShipment;
  shipmentItems$: Observable<ListShipmentItem>;
  lineItems: ListLineItem;

  constructor(private activatedRoute: ActivatedRoute, private context: ShopperContextService) {}

  ngOnInit() {
    combineLatest(this.activatedRoute.parent.data, this.activatedRoute.data)
      .pipe(map((results) => [results[0].orderResolve.lineItems, results[1].shipmentsResolve]))
      .subscribe(([lineItems, shipments]) => {
        this.lineItems = lineItems;
        this.shipments = shipments;
        if (this.shipments.Items.length) {
          this.selectShipment(this.shipments.Items[0]);
        }
      });
  }

  selectShipment(shipment: Shipment): void {
    this.selectedShipment = { ...shipment };
    this.shipmentItems$ = this.context.myResources
      .ListShipmentItems(shipment.ID)
      .pipe(map((shipmentItems) => this.setLineItem(shipmentItems)));
  }

  private setLineItem(shipmentItems: ListShipmentItem) {
    // shipmentItems.Items.map((item) => {
    //   const lineItem = _find(this.lineItems.Items, (li) => li.ID === item.LineItemID);
    //   item.LineItem = lineItem;
    // });
    return shipmentItems;
  }
}
