import { Component, Inject, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { faShippingFast, faWindowClose, faPlus, faCog, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { LineItem, Shipment, OcSupplierAddressService, ListAddress, ListShipment, OcShipmentService, ListShipmentItem, OcLineItemService, OcOrderService, Order } from '@ordercloud/angular-sdk';
import { getProductMainImageUrlOrPlaceholder } from '@app-seller/products/product-image.helper';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppAuthService } from '@app-seller/auth';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OrderService } from '@app-seller/orders/order.service';
import { SELLER } from '@app-seller/shared/models/ordercloud-user.types';

@Component({
  selector: 'app-order-shipments',
  templateUrl: './order-shipments.component.html',
  styleUrls: ['./order-shipments.component.scss']
})
export class OrderShipmentsComponent implements OnChanges {
  faShippingFast = faShippingFast;
  faPlus = faPlus;
  faWindowClose = faWindowClose;
  faCog = faCog; // TO-DO - Use for edit button for Ship From address.
  createShipment = false;
  viewShipments = false;
  editShipFromAddress = false; // TO-DO - Use for editing Ship From address.
  shipmentForm: FormGroup;
  shipments: ListShipment;
  shipmentItems: ListShipmentItem;
  selectedShipment: Shipment;
  supplierAddresses: ListAddress;
  quantities: number[] = [];
  lineItems: LineItem[];
  isSaving = false;
  isSellerUser = false;
  @Input()
  orderDirection: string;
  @Input()
  order: Order;
  @Output()
  createOrViewShipmentEvent = new EventEmitter<boolean>();

  constructor(
    private orderService: OrderService,
    private ocOrderService: OcOrderService,
    private ocSupplierAddressService: OcSupplierAddressService,
    private ocShipmentService: OcShipmentService,
    private ocLineItemService: OcLineItemService,
    private httpClient: HttpClient,
    private appAuthService: AppAuthService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
    ) { 
      this.isSellerUser = this.appAuthService.getOrdercloudUserType() === SELLER;
    }

  ngOnChanges() {
    if (this.order.ID) {
      this.createShipment = false;
      this.getShipments();
      this.getLineItems();
    }
  }

  setShipmentForm(): void {
    this.shipmentForm = new FormGroup({
      TrackingNumber: new FormControl(''),
      ShipDate: new FormControl(this.getCurrentDate(), Validators.required),
      Cost: new FormControl(''),
      // TO-DO: Use below line of code when it's possible to POST a supplier's address ID
      // FromAddressID: new FormControl(''),
      Shipper: new FormControl(''),
      Service: new FormControl(''),
    });
  }

  getCurrentDate(): string {
    // Get local date of user, then format to lead with year and leading 0's for single-digit months/days
    const date = new Date().toLocaleDateString();
    const newDate = date.split('/');
    if (newDate[0].length === 1) {
      newDate[0] = '0' + newDate[0];
    }
    if (newDate[1].length === 1) {
      newDate[1] = '0' + newDate[1];
    }
    return newDate[2] + '-' + newDate[0] + '-' + newDate[1];
  }

  toggleCreateShipment(): void {
    this.populateLineItemQuantities(this.lineItems);
    this.getSupplierAddresses();
    this.setShipmentForm();
    this.createShipment = !this.createShipment;
    this.viewShipments = false;
    this.createOrViewShipmentEvent.emit(this.createShipment);
  }

  canCreateShipment(): boolean {
    const unshippedItem = this.lineItems.find(item => item.Quantity > item.QuantityShipped);
    return unshippedItem ? true : false;
  }

  isQuoteOrder(order: Order) {
    return this.orderService.isQuoteOrder(order);
  }

  toggleViewShipments(): void {
    this.setSelectedShipment(0);
    this.viewShipments = !this.viewShipments;
    this.createShipment = false;
    this.createOrViewShipmentEvent.emit(this.viewShipments);
  }

  // TO-DO - Use commented code when able to POST supplier Ship From addresses
  // toggleEditShipFromAddress() {
  //   this.editShipFromAddress = !this.editShipFromAddress;
  // }
  
  // handleUpdateShipFromAddress(addressID) {
  // }

  async getShipments(): Promise<void> {
    const shipments = await this.ocOrderService.ListShipments(this.orderDirection, this.order.ID).toPromise();
    this.shipments = shipments;
  }

  async getLineItems(): Promise<void> {
    const lineItemsResponse = await this.ocLineItemService.List(this.orderDirection, this.order.ID).toPromise();
    this.lineItems = lineItemsResponse.Items;
  }

  setSelectedShipment(i: number): void {
    this.selectedShipment = this.shipments.Items[i];
    this.getShipmentItems(this.selectedShipment.ID);
  }

  async getShipmentItems(shipmentID: string): Promise<void> {
    this.shipmentItems = await this.ocShipmentService.ListItems(shipmentID).toPromise();
 }

  populateLineItemQuantities(lineItems: LineItem[]): void {
    this.quantities = Array(lineItems.length).fill(0);
  }

  getImageUrl(lineItem: LineItem): string {
    const product = lineItem.Product;
    return getProductMainImageUrlOrPlaceholder(product);
  }

  getCreateButtonAction(): string {
    return this.createShipment ? 'Cancel' : 'Create';
  }

  getViewButtonAction(): string {
    return this.viewShipments ? 'Hide' : 'View';
  }

  // TO-DO - Use commented code for Ship From Address POST
  // getEditShipFromAddressButtonAction() {
  //   return this.editShipFromAddress ? 'Cancel' : 'Edit Ship From Address';
  // }

  getCreateButtonIcon(): IconDefinition {
    return this.createShipment ? faWindowClose : faPlus;
  }

  getViewButtonIcon(): IconDefinition {
    return this.viewShipments ? faWindowClose : faShippingFast;
  }

   // TO-DO - Use commented code for Ship From Address POST
  // getEditShipFromAddressButtonIcon() {
  //   return this.editShipFromAddress ? faWindowClose : faCog;
  // }

  async getSupplierAddresses(): Promise<void> {
    if (!this.supplierAddresses) {
      this.supplierAddresses = await this.ocSupplierAddressService.List(this.order.ToCompanyID).toPromise();
    }
  }

  getQuantityDropdown(quantityToShip: number): number[] {
    const quantityList = [];
    for (let i = 1; i <= quantityToShip; i++) {
      quantityList.push(i);
    }
    return quantityList;
  }

  setQuantityToShip(i: number, quantity: number): void {
    this.quantities[i] = quantity;
  }

  async onSubmit(): Promise<void> {
    this.isSaving = true;
    const shipDate = this.shipmentForm.value.ShipDate;
    this.shipmentForm.value.ShipDate = null;
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type':  'application/json',
        Authorization: 'Bearer ' + accessToken
      })
    };
    const superShipment = {
      Shipment: {...this.shipmentForm.value, xp: {Service: this.shipmentForm.value.Service}},
      ShipmentItems: this.lineItems.map((li, i) => {
        if (this.quantities[i] > 0) {
          return { LineItemID: li.ID, OrderID: this.order.ID, QuantityShipped: this.quantities[i] }
        }
      }).filter(li => li !== undefined)
    }
    const postedShipment: any = await this.httpClient.post(this.appConfig.middlewareUrl + '/shipment', superShipment, httpOptions ).toPromise();
    await this.ocShipmentService.Patch(postedShipment.Shipment.ID, {DateShipped: shipDate}).toPromise();
    this.getShipments();
    this.getLineItems();
    this.createShipment = false;
    this.isSaving = false;
  }

  shouldDisableSave(shipment: FormGroup): boolean {
    if (shipment.value.TrackingNumber === '') return true;
    if (shipment.value.ShipDate === '') return true;
    if (shipment.value.Shipper === '') return true;
    const validQuantity = this.quantities.find(qty => qty > 0);
    if (!validQuantity) return true;
    if (this.isSaving) return true;
  }
}
