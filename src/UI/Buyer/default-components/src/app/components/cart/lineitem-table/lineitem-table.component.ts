import { Component, Input, OnInit } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { Address, LineItem, OcSupplierAddressService, OcSupplierService, Supplier } from '@ordercloud/angular-sdk';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService } from 'marketplace';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';

@Component({
  templateUrl: './lineitem-table.component.html',
  styleUrls: ['./lineitem-table.component.scss'],
})
export class OCMLineitemTable implements OnInit {
  closeIcon = faTimes;
  @Input() lineItems: LineItem[];
  @Input() readOnly: boolean;
  supplierInfo: Supplier[] = [];
  supplierAddresses: Address[] = [];
  liGroupedByShipFrom: LineItem[][];
  liGroups: any;

  constructor(private context: ShopperContextService, private ocSupplierService: OcSupplierService, private ocSupplierAddressService: OcSupplierAddressService) { }

  async ngOnInit() {
    await this.lineItems;
    this.liGroups = _groupBy(this.lineItems, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(this.liGroups);
    this.supplierInfo = await this.getSupplierInfo();
    this.supplierAddresses = await this.getSupplierAddresses();
  }

  getSupplierInfo() {
    let infoArray = [];
    this.liGroupedByShipFrom.forEach(async group => {
      if (group[0] && group[0].SupplierID) {
        let info = await this.ocSupplierService.Get(group[0].SupplierID).toPromise();
        infoArray.push(info);
      }
    });
    return infoArray;
  }

  getSupplierAddresses() {
    let addresses = [];
    this.liGroupedByShipFrom.forEach(async group => {
      if (group[0] && group[0].SupplierID && group[0].ShipFromAddressID) {
        let address = await this.ocSupplierAddressService.Get(group[0].SupplierID, group[0].ShipFromAddressID).toPromise();
        addresses.push(address);
      }
    });
    return addresses;
  }

  removeLineItem(lineItemID: string) {
    this.context.currentOrder.removeFromCart(lineItemID);
  }

  toProductDetails(productID: string) {
    this.context.router.toProductDetails(productID);
  }

  changeQuantity(lineItemID: string, event: { qty: number; valid: boolean }) {
    if (event.valid) {
      this.getLineItem(lineItemID).Quantity = event.qty;
      this.context.currentOrder.setQuantityInCart(lineItemID, event.qty);
    }
  }

  getImageUrl(lineItemID: string) {
    const li = this.getLineItem(lineItemID);
    return getPrimaryImageUrl(li.Product);
  }

  getLineItem(lineItemID: string): LineItem {
    return this.lineItems.find(li => li.ID === lineItemID);
  }
}
