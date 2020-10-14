import { Component, Input, OnInit } from '@angular/core';
import { faTimes, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService, LineItemGroupSupplier, OrderType } from 'marketplace';
import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';
import { QtyChangeEvent } from '../../products/quantity-input/quantity-input.component';
import { getPrimaryLineItemImage } from 'src/app/services/images.helpers';
import { CancelReturnReason } from '../../orders/order-return/order-return-table/models/cancel-return-translations.enum';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  templateUrl: './lineitem-table.component.html',
  styleUrls: ['./lineitem-table.component.scss'],
})
export class OCMLineitemTable implements OnInit {
  closeIcon = faTimes;
  faTrashAlt = faTrashAlt;
  @Input() set lineItems(lineItems: MarketplaceLineItem[]) {
    this._lineItems = lineItems;
  }
  @Input() set groupByKits(bool: boolean) {
    this._groupByKits = bool;
    this.liGroupedByShipFrom = this.groupLineItemsByShipFrom(this._lineItems);
    this.liGroupedByKit = this.groupLineItemsByKitID(this._lineItems);
    this.setSupplierInfo(this.liGroupedByShipFrom);
  }
  @Input() readOnly: boolean;
  @Input() orderType: OrderType;
  @Input() hideStatus = false;
  suppliers: LineItemGroupSupplier[];
  liGroupedByShipFrom: MarketplaceLineItem[][];
  liGroupedByKit: MarketplaceLineItem[][];
  updatingLiIDs: string[] = [];
  _groupByKits: boolean;
  _lineItems = [];
  _orderCurrency: string;
  showKitDetails = true;
  constructor(private context: ShopperContextService, private spinner: NgxSpinnerService) {
    this._orderCurrency = this.context.currentUser.get().Currency;
  }

  ngOnInit(): void {
    this.spinner.show(); // visibility is handled by *ngIf
  }

  toggleKitDetails(): void {
    this.showKitDetails = !this.showKitDetails;
  }

  groupLineItemsByKitID(lineItems: MarketplaceLineItem[]): MarketplaceLineItem[][] {
    if (!this._groupByKits) return [];
    const kitLineItems = lineItems.filter(li => li.xp.KitProductID);
    const liKitGroups = _groupBy(kitLineItems, li => li.xp.KitProductID);
    return Object.values(liKitGroups);
  }

  groupLineItemsByShipFrom(lineItems: MarketplaceLineItem[]): MarketplaceLineItem[][] {
    const supplierLineItems = this._groupByKits ? lineItems.filter(li => !li.xp.KitProductID) : lineItems;
    const liGroups = _groupBy(supplierLineItems, li => li.ShipFromAddressID);
    return Object.values(liGroups)
      .sort((a, b) => {
        const nameA = a[0].ShipFromAddressID.toUpperCase(); // ignore upper and lowercase
        const nameB = b[0].ShipFromAddressID.toUpperCase(); // ignore upper and lowercase
        return nameA.localeCompare(nameB);
      });
  }

  async setSupplierInfo(liGroups: MarketplaceLineItem[][]): Promise<void> {
    this.suppliers = await this.context.orderHistory.getLineItemSuppliers(liGroups);
  }

  removeLineItem(lineItemID: string): void {
    this.context.order.cart.remove(lineItemID);
  }

  removeKit(kit: any): void {
    this.context.order.cart.removeMany(kit)
  }

  toProductDetails(productID: string): void {
    this.context.router.toProductDetails(productID);
  }

  async changeQuantity(lineItemID: string, event: QtyChangeEvent): Promise<void> {
    if (event.valid) {
      const li = this.getLineItem(lineItemID);
      li.Quantity = event.qty;
      const { ProductID, Specs, Quantity, xp } = li;
      // ACTIVATE SPINNER/DISABLE INPUT IF QTY BEING UPDATED
      this.updatingLiIDs.push(lineItemID);
      await this.context.order.cart.setQuantity({ ProductID, Specs, Quantity, xp });
      // REMOVE SPINNER/ENABLE INPUT IF QTY NO LONGER BEING UPDATED
      this.updatingLiIDs.splice(this.updatingLiIDs.indexOf(lineItemID), 1);
    }
  }

  isQtyChanging(lineItemID: string): boolean {
    return this.updatingLiIDs.includes(lineItemID);
  }

  getImageUrl(lineItemID: string): string {
    return getPrimaryLineItemImage(lineItemID, this._lineItems, this.context.currentUser.get())
  }

  getLineItem(lineItemID: string): MarketplaceLineItem {
    return this._lineItems.find(li => li.ID === lineItemID);
  }

  hasReturnInfo(): boolean {
    return this._lineItems.some(li => !!li.xp?.LineItemReturnInfo);
  }

  hasCancelInfo(): boolean {
    return this._lineItems.some(li => !!li.xp?.LineItemCancelInfo);
  }

  getReturnReason(reasonCode: string): string {
    return CancelReturnReason[reasonCode];
  }
}
