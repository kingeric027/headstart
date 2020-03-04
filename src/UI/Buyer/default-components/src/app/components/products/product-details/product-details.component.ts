import { Component, Input, OnInit } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { ListSpec, OcLineItemService, OcOrderService, Order, User } from '@ordercloud/angular-sdk';
import { minBy as _minBy } from 'lodash';
import { LineItem, MarketplaceOrder, MarketplaceProduct, OrderType, ProductType, ShopperContextService } from 'marketplace';
import { Observable } from 'rxjs';
import { ModalState } from 'src/app/models/modal-state.class';
import { getImageUrls } from 'src/app/services/images.helpers';
import { SpecFormService } from '../spec-form/spec-form.service';

@Component({
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class OCMProductDetails implements OnInit {
  faTimes = faTimes;
  _specs: ListSpec;
  _product: MarketplaceProduct;
  specFormService: SpecFormService;
  isOrderable = false;
  quantity: number;
  price: number;
  percentSavings: number;
  priceBreaks: object;
  priceBreakRange: string[];
  selectedBreak: object;
  relatedProducts$: Observable<MarketplaceProduct[]>;
  imageUrls: string[] = [];
  favoriteProducts: string[] = [];
  qtyValid = true;
  supplierNote: string;
  specLength: number;
  quoteFormModal = ModalState.Closed;
  currentUser: User;
  showRequestSubmittedMessage = false;
  submittedQuoteOrder: Order;
  defaultQuoteOrder: MarketplaceOrder;
  lineItem: LineItem = {};

  constructor(
    private formService: SpecFormService,
    private context: ShopperContextService,
    private ocOrderService: OcOrderService,
    private ocLineItemService: OcLineItemService) {
    this.specFormService = formService;
  }

  @Input() set specs(value: ListSpec) {
    this._specs = value;
    this.specFormService.event.valid = this._specs.Items.length === 0;
    this.specLength = this._specs.Items.length;
  }

  @Input() set product(value: MarketplaceProduct) {
    this._product = value;
    this.isOrderable = !!this._product.PriceSchedule;
    this.imageUrls = this.getImageUrls();
    this.supplierNote = this._product.xp && this._product.xp.Note;
  }

  ngOnInit(): void {
    this.currentUser = this.context.currentUser.get();
    this.context.currentUser.onChange(user => (this.favoriteProducts = user.FavoriteProductIDs));
    this.defaultQuoteOrder = {
      xp: {
        AvalaraTaxTransactionCode: '',
        OrderType: OrderType.Quote,
        QuoteOrderInfo: {
          FirstName: '',
          LastName: '',
          Phone: '',
          Email: '',
          Comments: ''
        }
      },
    };
  }

  onSpecFormChange(event): void {
    if (event.detail.type === 'Change') {
      this.specFormService.event = event.detail;
      this.price = this.getTotalPrice();
    }
  }

  openQuoteForm() {
    this.quoteFormModal = ModalState.Open;
  }

  isQuoteProduct(): boolean {
    return this._product.xp.ProductType === ProductType.Quote;
  }

  qtyChange(event: { qty: number; valid: boolean }): void {
    if (event.valid) {
      this.quantity = event.qty;
      this.price = this.getTotalPrice();
    }
  }

  addToCart(): void {
    this.context.order.cart.add({
      ProductID: this._product.ID,
      Quantity: this.quantity,
      Specs: this.specFormService.getLineItemSpecs(this._specs),
    });
  }

  getPriceBreakRange(index: number): string {
    if (!this._product.PriceSchedule?.PriceBreaks.length) return '';

    const priceBreaks = this._product.PriceSchedule.PriceBreaks;
    const indexOfNextPriceBreak = index + 1;
    if (indexOfNextPriceBreak < priceBreaks.length) {
      return `${priceBreaks[index].Quantity} - ${priceBreaks[indexOfNextPriceBreak].Quantity - 1}`;
    } else {
      return `${priceBreaks[index].Quantity}+`;
    }
  }

  getTotalPrice(): number {
    // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
    // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
    // is the cost per item.
    if (!this._product.PriceSchedule?.PriceBreaks.length) return;

    const priceBreaks = this._product.PriceSchedule.PriceBreaks;
    this.priceBreaks = priceBreaks;
    const startingBreak = _minBy(priceBreaks, 'Quantity');
    const selectedBreak = priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity && candidate.Quantity <= this.quantity ? candidate : current;
    }, startingBreak);
    this.selectedBreak = selectedBreak;
    this.percentSavings = parseInt(
      (((priceBreaks[0].Price - selectedBreak.Price) / priceBreaks[0].Price) * 100).toFixed(0), 10
    );
    return this.specFormService.event.valid
      ? this.specFormService.getSpecMarkup(this._specs, selectedBreak, this.quantity || startingBreak.Quantity)
      : selectedBreak.Price * (this.quantity || startingBreak.Quantity);
  }

  getImageUrls(): string[] {
    return getImageUrls(this._product);
  }

  isFavorite(): boolean {
    return this.favoriteProducts.includes(this._product.ID);
  }

  setIsFavorite(isFav: boolean): void {
    this.context.currentUser.setIsFavoriteProduct(isFav, this._product.ID);
  }

  setActiveSupplier(supplierId: string): void {
    this.context.router.toProductList({ activeFacets: { Supplier: supplierId.toLowerCase() } });
  }

  dismissQuoteForm() {
    this.quoteFormModal = ModalState.Closed;
  }

  async submitQuoteOrder(user) {
    this.defaultQuoteOrder.xp.QuoteOrderInfo = user;
    this.lineItem.ProductID = this._product.ID;
    this.lineItem.Product = this._product;
    this.submittedQuoteOrder = await this.ocOrderService.Create('Outgoing', this.defaultQuoteOrder).toPromise();
    await this.ocLineItemService.Create('Outgoing', this.submittedQuoteOrder.ID, this.lineItem).toPromise();
    this.quoteFormModal = ModalState.Closed
    this.showRequestSubmittedMessage = true
  }

  toOrderDetail() {
    this.context.router.toMyOrderDetails(this.submittedQuoteOrder.ID);
  }
}
