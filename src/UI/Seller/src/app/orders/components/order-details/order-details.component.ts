import { Component, Input, Inject } from '@angular/core';
import { OrderService } from '@app-seller/orders/order.service';
import { Address, LineItem, OcLineItemService, OcOrderService, OcPaymentService, Order, Payment } from '@ordercloud/angular-sdk';
import { groupBy as _groupBy } from 'lodash';
import { ProductImage } from 'marketplace-javascript-sdk';
import { PDFService } from '@app-seller/orders/pdf-render.service';
import { faDownload, faUndo } from '@fortawesome/free-solid-svg-icons';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { SELLER } from '@app-seller/shared/models/ordercloud-user.types';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { AppAuthService } from '@app-seller/auth';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss'],
})
export class OrderDetailsComponent {
  faDownload = faDownload;
  faUndo = faUndo;
  _order: Order = {};
  _lineItems: LineItem[] = [];
  _payments: Payment[] = [];
  _liGroupedByShipFrom: LineItem[][];
  _liGroups: any;
  images: ProductImage[] = [];
  orderDirection: string;
  cardType: string;
  createShipment: boolean;
  isSellerUser = false;
  isSaving = false;

  @Input()
  set order(order: Order) {
    if (Object.keys(order).length) {
      this.createShipment = false;
      this.handleSelectedOrderChange(order);
    }
  }
  constructor(
    private ocLineItemService: OcLineItemService,
    private ocPaymentService: OcPaymentService,
    private orderService: OrderService,
    private pdfService: PDFService,
    private middleware: MiddlewareAPIService,
    private appAuthService: AppAuthService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    this.isSellerUser = this.appAuthService.getOrdercloudUserType() === SELLER;
  }

  setCardType(payment) {
    if (!payment.xp.cardType || payment.xp.cardType === null) {
      return 'Card';
    }
    this.cardType = payment.xp.cardType.charAt(0).toUpperCase() + payment.xp.cardType.slice(1);
    return this.cardType;
  }

  showReturnInfo() {
    return this._order?.xp?.OrderReturnInfo?.HasReturn && this.orderDirection === 'Incoming' && this.isSellerUser;
  }

  getFullName(address: Address) {
    const fullName = `${address.FirstName || ''} ${address.LastName || ''}`;
    return fullName.trim();
  }

  getIncomingOrOutgoing() {
    const url = window.location.href;
    url.includes('Outgoing') ? (this.orderDirection = 'Outgoing') : (this.orderDirection = 'Incoming');
  }

  async setOrderStatus() {
    await this.middleware.acknowledgeQuoteOrder(this._order.ID).then(completedOrder => this.handleSelectedOrderChange(completedOrder));
  }

  isQuoteOrder(order: Order) {
    return this.orderService.isQuoteOrder(order);
  }

  private async handleSelectedOrderChange(order: Order): Promise<void> {
    this._order = order;
    this.getIncomingOrOutgoing();
    const lineItemsResponse = await this.ocLineItemService.List(this.orderDirection, order.ID).toPromise();
    this._lineItems = lineItemsResponse.Items;
    const paymentsResponse = await this.ocPaymentService.List(this.orderDirection, order.ID).toPromise();
    this._payments = paymentsResponse.Items;
    this._liGroups = _groupBy(this._lineItems, li => li.ShipFromAddressID);
    this._liGroupedByShipFrom = Object.values(this._liGroups);
  }

  toggleCreateShipment(createShipment: boolean) {
    this.createShipment = createShipment;
  }

  protected createAndSavePDF(): void {
    this.pdfService.createAndSavePDF(this._order.ID);
  }
}