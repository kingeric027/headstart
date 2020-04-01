import { Component, Inject, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { faShippingFast, faWindowClose, faPlus, faCog } from '@fortawesome/free-solid-svg-icons';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { LineItem, Shipment, OcSupplierAddressService, ListAddress, ListShipment, OcShipmentService, ListShipmentItem, OcLineItemService } from '@ordercloud/angular-sdk';
import { getProductMainImageUrlOrPlaceholder } from '@app-seller/products/product-image.helper';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppAuthService } from '@app-seller/auth';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';

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
  @Input()
  order;
  @Output()
  createOrViewShipmentEvent = new EventEmitter<boolean>();

  constructor(
    private ocSupplierAddressService: OcSupplierAddressService,
    private ocShipmentService: OcShipmentService,
    private ocLineItemService: OcLineItemService,
    private formErrorService: AppFormErrorService,
    private httpClient: HttpClient,
    private appAuthService: AppAuthService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
    ) { }

  ngOnChanges() {
    if (this.order.ID) {
      this.getShipments();
      this.getLineItems();
    }
  }

  setShipmentForm() {
    this.shipmentForm = new FormGroup({
      TrackingNumber: new FormControl('', Validators.required),
      ShipDate: new FormControl(this.getCurrentDate(), Validators.required),
      Cost: new FormControl(''),
      // TO-DO: Use below line of code when it's possible to POST a supplier's address ID
      // FromAddressID: new FormControl(''),
      Shipper: new FormControl(''),
      ShippingService: new FormControl(''),
    });
  }

  getCurrentDate() {
    //Get local date of user, then format to lead with year and leading 0's for single-digit months/days
    let date = new Date().toLocaleDateString();
    let newDate = date.split('/');
    if (newDate[0].length === 1) {
      newDate[0] = '0' + newDate[0];
    }
    if (newDate[1].length === 1) {
      newDate[1] = '0' + newDate[1];
    }
    return newDate[2] + '-' + newDate[0] + '-' + newDate[1];
  }

  toggleCreateShipment() {
    this.populateLineItemQuantities(this.lineItems);
    this.getSupplierAddresses();
    this.setShipmentForm();
    this.createShipment = !this.createShipment;
    this.viewShipments = false;
    this.createOrViewShipmentEvent.emit(this.createShipment);
  }

  toggleViewShipments() {
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

  async getShipments() {
    this.shipments = await this.ocShipmentService.List({orderID: this.order.ID, sortBy: 'DateShipped'}).toPromise();
  }

  async getLineItems() {
    const lineItemsResponse = await this.ocLineItemService.List('Incoming', this.order.ID).toPromise();
    this.lineItems = lineItemsResponse.Items;
  }

  setSelectedShipment(i) {
    this.selectedShipment = this.shipments.Items[i];
    this.getShipmentItems(this.selectedShipment.ID);
  }

  async getShipmentItems(shipmentID) {
    this.shipmentItems = await this.ocShipmentService.ListItems(shipmentID).toPromise();
 }

  populateLineItemQuantities(lineItems) {
    this.quantities = Array(lineItems.length).fill(0);
  }

  getImageUrl(lineItem: LineItem) {
    const product = lineItem.Product;
    return getProductMainImageUrlOrPlaceholder(product);
  }

  getCreateButtonAction() {
    return this.createShipment ? 'Cancel' : 'Create';
  }

  getViewButtonAction() {
    return this.viewShipments ? 'Hide' : 'View';
  }

  // TO-DO - Use commented code for Ship From Address POST
  // getEditShipFromAddressButtonAction() {
  //   return this.editShipFromAddress ? 'Cancel' : 'Edit Ship From Address';
  // }

  getCreateButtonIcon() {
    return this.createShipment ? faWindowClose : faPlus;
  }

  getViewButtonIcon() {
    return this.viewShipments ? faWindowClose : faShippingFast;
  }

   // TO-DO - Use commented code for Ship From Address POST
  // getEditShipFromAddressButtonIcon() {
  //   return this.editShipFromAddress ? faWindowClose : faCog;
  // }

  async getSupplierAddresses() {
    if (!this.supplierAddresses) {
      this.supplierAddresses = await this.ocSupplierAddressService.List(this.order.ToCompanyID).toPromise();
    }
  }

  getQuantityDropdown(quantityToShip) {
    let quantityList = [];
    for (let i = 1; i <= quantityToShip; i++) {
      quantityList.push(i);
    }
    return quantityList;
  }

  setQuantityToShip(i, quantity) {
    this.quantities[i] = quantity;
  }

  async onSubmit() {
    const shipDate = this.shipmentForm.value.ShipDate;
    this.shipmentForm.value.ShipDate = null;
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type':  'application/json',
        'Authorization': 'Bearer ' + accessToken
      })
    };
    const superShipment = {
      Shipment: {...this.shipmentForm.value, xp: {ShippingService: this.shipmentForm.value.ShippingService}},
      ShipmentItems: this.lineItems.map((li, i) => {
        if (this.quantities[i] > 0) {
          return { LineItemID: li.ID, OrderID: this.order.ID, QuantityShipped: this.quantities[i] }
        }
      }).filter(li => li !== undefined)
    }
    let postedShipment:any = await this.httpClient.post(this.appConfig.middlewareUrl + '/shipment', superShipment, httpOptions ).toPromise();
    await this.ocShipmentService.Patch(postedShipment.Shipment.ID, {DateShipped: shipDate}).toPromise();
    this.getShipments();
    this.getLineItems();
    this.createShipment = false;
  }
}
