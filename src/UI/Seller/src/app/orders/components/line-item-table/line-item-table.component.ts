import { Component, Input, Inject, Output, EventEmitter, OnInit } from '@angular/core';
import { groupBy as _groupBy } from 'lodash';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { MarketplaceLineItem, HeadStartSDK, MarketplaceOrder } from '@ordercloud/headstart-sdk';
import { LineItemTableStatus } from '../order-details/order-details.component';
import { NumberCanChangeTo, CanChangeTo, CanChangeLineItemsOnOrderTo } from '@app-seller/orders/line-item-status.helper';
import { LineItemStatus } from '@app-seller/shared/models/order-status.interface';
import { FormArray, Validators, FormControl } from '@angular/forms';
import { getPrimaryLineItemImage } from '@app-seller/products/product-image.helper';
import { MeUser, OcOrderService } from '@ordercloud/angular-sdk';
import { LineItem, LineItemSpec } from 'ordercloud-javascript-sdk';
import { RegexService } from '@app-seller/shared';

@Component({
  selector: 'app-line-item-table',
  templateUrl: './line-item-table.component.html',
  styleUrls: ['./line-item-table.component.scss'],
})
export class LineItemTableComponent {
  _lineItems: MarketplaceLineItem[] = [];
  _order: MarketplaceOrder;
  _liGroupedByShipFrom: MarketplaceLineItem[][];
  _supplierOrders: MarketplaceOrder[] = [];
  _statusChangeForm = new FormArray([]);
  _tableStatus = LineItemTableStatus.Default;
  _user: MeUser;
  @Input()
  set order(value: MarketplaceOrder) {
    this._order = value;
    this.setSupplierOrders(value);
  }
  @Input() orderDirection: 'Incoming' | 'Outgoing';
  @Output() orderChange = new EventEmitter();
  isSaving = false;

  @Input()
  set lineItems(value: MarketplaceLineItem[]) {
    this._lineItems = value;
    if (value?.length) {
      this.setLineItemGroups(value);
    }
  }

  constructor(
    private ocOrderService: OcOrderService,
    private regexService: RegexService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) { }

  changeTableStatus(newStatus: string): void {
    this._tableStatus = newStatus;
    if (this._tableStatus !== 'Default') {
      this.setupForm();
    } else {
      this.setLineItemGroups(this._lineItems);
    }
  }

  getShipMethodString(lineItem: MarketplaceLineItem): string {
    const salesOrderID = this._order.ID.split('-')[0];
    const supplierOrder = this._supplierOrders?.find(order => order.ID === `${salesOrderID}-${lineItem.SupplierID}`)
    const shipFromID = lineItem.ShipFromAddressID;
    const shipMethod = (supplierOrder?.xp?.SelectedShipMethodsSupplierView || [])
      .find(sm => sm.ShipFromAddressID === shipFromID);
    if (shipMethod == null) return 'No Data';
    const name = shipMethod.Name.replace(/_/g, ' ');
    const delivery = new Date(this._order.DateSubmitted);
    delivery.setDate(delivery.getDate() + shipMethod.EstimatedTransitDays);
    return `${name}, ${delivery.toLocaleDateString('en-US')} Delivery`;
  }

  setupForm(): void {
    this.filterOutNonChangeables();
    const shipFromFormArrays = this._liGroupedByShipFrom.map(shipFrom => {
      const controls = shipFrom.map(li => {
        return new FormControl(0, [Validators.min(0), Validators.max(NumberCanChangeTo(this._tableStatus as LineItemStatus, li))])
      })
      return new FormArray(controls)
    })
    this._statusChangeForm = new FormArray(shipFromFormArrays);
  }

  filterOutNonChangeables(): void {
    const filteredLineItems = this._lineItems.filter(li => CanChangeTo(this._tableStatus as LineItemStatus, li));
    this.setLineItemGroups(filteredLineItems);
  }

  setLineItemGroups(lineItems: MarketplaceLineItem[]): void {
    const liGroups = _groupBy(lineItems, li => li.ShipFromAddressID);
    this._liGroupedByShipFrom = Object.values(liGroups);
  }

  async setSupplierOrders(order: MarketplaceOrder): Promise<void> {
    const salesOrderID = order?.ID?.split('-')[0];
    if (order.ID === salesOrderID) {
      const supplierOrderFilterString = order?.xp?.SupplierIDs?.map(id => `${order.ID}-${id}`).join('|');
      const supplierOrders = await this.ocOrderService.List(this.orderDirection === 'Incoming' ? 'Outgoing' : 'Incoming',
        { filters: { ID: supplierOrderFilterString } }).toPromise();
      this._supplierOrders = supplierOrders.Items;
    } else {
      this._supplierOrders = [order];
    }
  }

  canChangeTo(lineItemStatus: LineItemStatus): boolean {
    return CanChangeLineItemsOnOrderTo(lineItemStatus, this._lineItems);
  }

  getVariableTextSpecs = (li: LineItem): LineItemSpec[] => li?.Specs?.filter(s => s.OptionID === null);

  getLineItemStatusDisplay(lineItem: MarketplaceLineItem): string {
    return Object.entries(lineItem.xp.StatusByQuantity)
      .filter(([status, quantity]) => quantity)
      .map(([status, quantity]) => {
        const readableStatus = this.regexService.getStatusSplitByCapitalLetter(status);
        return `${quantity} ${readableStatus}`;
      })
      .join(', ');
  }

  getReturnReason(reason: string): string {
    return this.regexService.getStatusSplitByCapitalLetter(reason);
  }

  quantityCanChange(lineItem: MarketplaceLineItem): number {
    return NumberCanChangeTo(this._tableStatus as LineItemStatus, lineItem);
  }

  areChanges(): boolean {
    return this._statusChangeForm.controls.some(control => {
      return (control as any).controls.some(subControl => subControl.value > 0);
    })
  }

  async saveChanges(): Promise<void> {
    this.isSaving = true;
    try {
      const lineItemChanges = this.buildLineItemChanges();
      await HeadStartSDK.Orders.SellerSupplierUpdateLineItemStatusesWithNotification(this._order.ID, this.orderDirection, lineItemChanges);
      this.orderChange.emit();
      this.changeTableStatus('Default')
      this.isSaving = false;
    } catch (ex) {
      this.isSaving = false;
      throw ex;
    }
  }

  // temporarily qny
  // buildLineItemChanges(): LineItemStatusChanges {
  buildLineItemChanges(): any {
    // const lineItemChanges: LineItemStatusChanges = {
    const lineItemChanges: any = {
      Status: this._tableStatus as LineItemStatus,
      Changes: []
    }

    this._statusChangeForm.controls.forEach((control, shipFromIndex) => {
      (control as any).controls.forEach((subControl, lineItemIndex) => {
        if (control.value) {
          const lineItem = this._liGroupedByShipFrom[shipFromIndex][lineItemIndex];
          lineItemChanges.Changes.push({
            ID: lineItem.ID,
            Quantity: subControl.value
          });
        }
      })
    });

    return lineItemChanges;
  }

  getImageUrl(lineItemID: string): string {
    return getPrimaryLineItemImage(lineItemID, this._lineItems, this.appConfig.sellerID)
  }
}
