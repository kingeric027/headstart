import { Component, OnInit } from '@angular/core';
import { faCube, faTruck } from '@fortawesome/free-solid-svg-icons';
import { ShopperContextService, OrderReorderResponse, OrderViewContext, ShippingStatus } from 'marketplace';
import { MarketplaceOrder, OrderDetails, MarketplaceLineItem } from '@ordercloud/headstart-sdk';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { isQuoteOrder } from '../../../services/orderType.helper';

@Component({
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OCMOrderDetails implements OnInit {
  order: MarketplaceOrder;
  orderDetails: OrderDetails;
  approvalVersion = false;
  faCube = faCube;
  faTruck = faTruck;
  subView: 'details' | 'shipments' = 'details';
  reorderResponse: OrderReorderResponse;
  message = { string: null, classType: null };
  showRequestReturn = false;
  showRequestCancel = false;
  isQuoteOrder = isQuoteOrder;
  constructor(private context: ShopperContextService, private modalService: NgbModal) {}

  async ngOnInit(): Promise<void> {
    this.approvalVersion = this.context.router.getOrderViewContext() === OrderViewContext.Approve;
    this.orderDetails = await this.context.orderHistory.getOrderDetails();
    this.order = this.orderDetails.Order;
    this.validateReorder(this.order.ID, this.orderDetails.LineItems);
  }

  open(content: HTMLTemplateElement): void {
    if (this.reorderResponse) {
      this.modalService.open(content, { ariaLabelledBy: 'confirm-modal' });
    }
  }

  async validateReorder(orderID: string, lineItems: MarketplaceLineItem[]): Promise<void> {
    this.reorderResponse = await this.context.orderHistory.validateReorder(orderID, lineItems);
    this.updateMessage(this.reorderResponse);
  }

  isFavorite(orderID: string): boolean {
    return this.context.currentUser.get().FavoriteOrderIDs.includes(orderID);
  }

  canRequestReturn(): boolean {
    return this.orderDetails.LineItems.some(li => (li as any).xp.StatusByQuantity?.Complete);
  }

  canRequestCancel(): boolean {
    return this.orderDetails.LineItems.some(li => (li as any).xp.StatusByQuantity?.Submitted || (li as any).xp.StatusByQuantity?.Backordered);
  }

  toggleFavorite(order: MarketplaceOrder): void {
    const newValue = !this.isFavorite(order.ID);
    this.context.currentUser.setIsFavoriteOrder(newValue, order.ID);
  }

  toggleRequestReturn(): void {
    this.showRequestReturn = !this.showRequestReturn;
    if(this.showRequestReturn) this.showRequestCancel = false;
  }

  toggleRequestCancel(): void {
    this.showRequestCancel = !this.showRequestCancel;
    if(this.showRequestCancel) this.showRequestReturn = false;
  }

  toShipments(): void {
    this.subView = 'shipments';
  }

  toDetails(): void {
    this.subView = 'details';
  }

  toAllOrders(): void {
    this.context.router.toMyOrders();
  }

  showShipments(): boolean {
    return this.subView === 'shipments';
  }

  showDetails(): boolean {
    return this.subView === 'details';
  }

  updateMessage(response: OrderReorderResponse): void {
    if (response.InvalidLi.length && !response.ValidLi.length) {
      this.message.string = 'None of the line items on this order are available for reorder.';
      this.message.classType = 'danger';
      return;
    }
    if (response.InvalidLi.length && response.ValidLi.length) {
      this.message.string =
        '<strong>Warning</strong> The following line items are not available for reorder, clicking add to cart will <strong>only</strong> add valid line items.';
      this.message.classType = 'warning';
      return;
    }
    this.message.string = 'All line items are valid to reorder';
    this.message.classType = 'success';
  }

  async addToCart(): Promise<void> {
    const items = this.reorderResponse.ValidLi.map(li => {
      return {
        ProductID: li.Product.ID,
        Quantity: li.Quantity,
        Specs: li.Specs,
        xp: {
          ImageUrl: li.xp?.ImageUrl,
        },
      };
    });
    await this.context.order.cart.addMany(items as any);
  }

  async moveOrderToCart(): Promise<void> {
    await this.context.order.cart.moveOrderToCart(this.order.ID);
  }

  toggleShowRequestForm(showRequestReturn: boolean): void {
    this.ngOnInit();
    this.showRequestReturn = showRequestReturn;
    this.showRequestCancel = showRequestReturn;
  }
}
