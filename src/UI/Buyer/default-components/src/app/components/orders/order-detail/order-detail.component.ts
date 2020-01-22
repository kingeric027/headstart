import { Component, OnInit } from '@angular/core';
import { faCube, faTruck } from '@fortawesome/free-solid-svg-icons';
import { OrderDetails, ShopperContextService, MarketplaceOrder, OrderReorderResponse } from 'marketplace';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OCMOrderDetails implements OnInit {
  order: MarketplaceOrder;
  orderDetails: OrderDetails;
  approvalVersion: boolean;
  faCube = faCube;
  faTruck = faTruck;
  subView: 'details' | 'shipments' = 'details';
  reorderResponse: OrderReorderResponse;
  message = { string: null, classType: null };

  constructor(private context: ShopperContextService, private modalService: NgbModal) { }

  async ngOnInit() {
    this.orderDetails = await this.context.orderHistory.getOrderDetails();
    this.order = this.orderDetails.order;
    const url = this.context.router.getActiveUrl();
    this.approvalVersion = url.includes('/approval');
    this.validateReorder(this.order.ID);
  }

  async open(content) {
    if (this.reorderResponse) {
      await this.modalService.open(content, { ariaLabelledBy: 'confirm-modal' });
    }
  }

  async validateReorder(orderID: string): Promise<void> {
    this.reorderResponse = await this.context.orderHistory.validateReorder(orderID);
    this.updateMessage(this.reorderResponse);
  }

  isFavorite(orderID: string): boolean {
    return this.context.currentUser.favoriteOrderIDs.includes(orderID);
  }

  toggleFavorite(order: MarketplaceOrder) {
    const newValue = !this.isFavorite(order.ID);
    this.context.currentUser.setIsFavoriteOrder(newValue, order.ID);
  }

  toShipments() {
    this.subView = 'shipments';
  }

  toDetails() {
    this.subView = 'details';
  }

  showShipments(): boolean {
    return this.subView === 'shipments';
  }

  showDetails(): boolean {
    return this.subView === 'details';
  }

  updateMessage(response: OrderReorderResponse): void {
    if (response.InvalidLi.length && !response.ValidLi.length) {
      this.message.string = `None of the line items on this order are available for reorder.`;
      this.message.classType = 'danger';
      return;
    }
    if (response.InvalidLi.length && response.ValidLi.length) {
      this.message.string = `<strong>Warning</strong> The following line items are not available for reorder, clicking add to cart will <strong>only</strong> add valid line items.`;
      this.message.classType = 'warning';
      return;
    }
    this.message.string = `All line items are valid to reorder`;
    this.message.classType = 'success';
  }

  async addToCart(): Promise<void> {
    const items = this.reorderResponse.ValidLi.map(li => {
      return { ProductID: li.Product.ID, Quantity: li.Quantity, Specs: li.Specs };
    });
    await this.context.currentOrder.addManyToCart(items);
  }
}
