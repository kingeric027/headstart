import { Component, Input, Inject } from '@angular/core';
import { OrderService } from '@app-seller/orders/order.service';
import {
  Address,
  LineItem,
  OcLineItemService,
  OcPaymentService,
  Order,
  Payment,
  OcOrderService,
  OrderDirection,
} from '@ordercloud/angular-sdk';
import { groupBy as _groupBy } from 'lodash';

// temporarily any with sdk update
// import { ProductImage } from '@ordercloud/headstart-sdk';
import { PDFService } from '@app-seller/orders/pdf-render.service';
import { faDownload, faUndo, faExclamationTriangle, faInfoCircle, faUserAlt } from '@fortawesome/free-solid-svg-icons';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { SELLER } from '@app-seller/shared/models/ordercloud-user.types';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { AppAuthService } from '@app-seller/auth';
import { ReturnReason } from '@app-seller/shared/models/return-reason.interface';
import { MarketplaceLineItem, MarketplaceOrder } from '@ordercloud/headstart-sdk';
import { LineItemStatus } from '@app-seller/shared/models/order-status.interface';
import { CanChangeLineItemsOnOrderTo } from '@app-seller/orders/line-item-status.helper';

export const LineItemTableStatus = {
  Default: 'Default',
  Canceled: 'Canceled',
  Returned: 'Returned',
  Backorered: 'Backorered',
};

export interface OrderProgress {
  StatusDisplay: string;
  Value: number;
  ProgressBarType: string;
  Striped: boolean;
  Animated: boolean;
}

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss'],
})
export class OrderDetailsComponent {
  faDownload = faDownload;
  faUndo = faUndo;
  faExclamationTriangle = faExclamationTriangle;
  faInfoCircle = faInfoCircle;
  faUser = faUserAlt;
  _order: Order = {};
  _lineItems: MarketplaceLineItem[] = [];
  _payments: Payment[] = [];
  images: any[] = [];
  orderDirection: OrderDirection;
  cardType: string;
  createShipment: boolean;
  isSellerUser = false;
  isSaving = false;
  orderProgress: OrderProgress = {
    StatusDisplay: 'Processing',
    Value: 25,
    ProgressBarType: 'primary',
    Striped: false,
    Animated: false
  };
  orderAvatarInitials: string;

  @Input()
  set order(order: Order) {
    if (Object.keys(order).length) {
      this.createShipment = false;
      this.handleSelectedOrderChange(order);
    }
  }
  constructor(
    private ocLineItemService: OcLineItemService,
    private ocOrderService: OcOrderService,
    private ocPaymentService: OcPaymentService,
    private orderService: OrderService,
    private pdfService: PDFService,
    private middleware: MiddlewareAPIService,
    private appAuthService: AppAuthService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    this.isSellerUser = this.appAuthService.getOrdercloudUserType() === SELLER;
  }

  setOrderProgress(order: MarketplaceOrder): void {
    switch(order?.xp?.ShippingStatus) {
      case 'Processing':
        this.orderProgress = { StatusDisplay: "Processing", Value: 25, ProgressBarType: 'primary', Striped: false, Animated: false }
        break;
      case 'PartiallyShipped':
        this.orderProgress = { StatusDisplay: "Partially Shipped", Value: 50, ProgressBarType: 'primary', Striped: false, Animated: false }
        break;
      case 'Backordered':
        this.orderProgress = { StatusDisplay: "Item Backordered", Value: 75, ProgressBarType: 'danger', Striped: true, Animated: true }
      case 'Shipped': 
        this.orderProgress = { StatusDisplay: "Complete", Value: 100, ProgressBarType: 'success', Striped: false, Animated: false }
        break;
    }
    if (order?.xp?.ClaimStatus === 'Pending') {
      this.orderProgress = { StatusDisplay: "Needs Attention", Value: 100, ProgressBarType: 'danger', Striped: true, Animated: true }
    }
    if (order?.xp?.SubmittedOrderStatus === 'Canceled') {
      this.orderProgress = { StatusDisplay: "Canceled", Value: 100, ProgressBarType: 'danger', Striped: false, Animated: false }
    }
  }

  setCardType(payment) {
    if (!payment.xp.cardType || payment.xp.cardType === null) {
      return 'Card';
    }
    this.cardType = payment.xp.cardType.charAt(0).toUpperCase() + payment.xp.cardType.slice(1);
    return this.cardType;
  }

  getReturnReason(reasonCode: string): string {
    return ReturnReason[reasonCode];
  }

  getFullName(address: Address) {
    const fullName = `${address?.FirstName || ''} ${address?.LastName || ''}`;
    return fullName.trim();
  }

  getIncomingOrOutgoing() {
    const url = window.location.href;
    url.includes('Outgoing') ? (this.orderDirection = 'Outgoing') : (this.orderDirection = 'Incoming');
  }

  async setOrderStatus() {
    await this.middleware
      .acknowledgeQuoteOrder(this._order.ID)
      .then(completedOrder => this.handleSelectedOrderChange(completedOrder));
  }

  isQuoteOrder(order: Order) {
    return this.orderService.isQuoteOrder(order);
  }

  private async handleSelectedOrderChange(order: Order): Promise<void> {
    this.orderAvatarInitials = !this.isQuoteOrder(order) ? `${order?.FromUser?.FirstName?.slice(0,1).toUpperCase()}${order?.FromUser?.LastName?.slice(0,1).toUpperCase()}`
      :
    `${order?.xp?.QuoteOrderInfo?.FirstName?.slice(0,1).toUpperCase()}${order?.xp?.QuoteOrderInfo?.LastName?.slice(0,1).toUpperCase()}`;
    this.setOrderProgress(order);
    this._order = order;
    this.getIncomingOrOutgoing();
    const lineItemsResponse = await this.ocLineItemService.List(this.orderDirection, order.ID).toPromise();
    this._lineItems = lineItemsResponse.Items as MarketplaceLineItem[];
    const paymentsResponse = await this.ocPaymentService.List(this.orderDirection, order.ID).toPromise();
    this._payments = paymentsResponse.Items;
  }

  async refreshOrder(): Promise<void> {
    const order = await this.ocOrderService.Get(this.orderDirection, this._order.ID).toPromise();
    this.handleSelectedOrderChange(order);
  }

  toggleCreateShipment(createShipment: boolean) {
    this.createShipment = createShipment;
  }

  protected createAndSavePDF(): void {
    this.pdfService.createAndSavePDF(this._order.ID);
  }
}
