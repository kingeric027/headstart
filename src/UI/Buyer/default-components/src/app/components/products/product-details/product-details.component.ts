import { Component, Input, OnInit } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { ListSpec, User } from '@ordercloud/angular-sdk';
import { minBy as _minBy } from 'lodash';
import { LineItem, MarketplaceMeProduct, OrderType, ShopperContextService } from 'marketplace';
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
  _product: MarketplaceMeProduct;
  specFormService: SpecFormService;
  isOrderable = false;
  quantity: number;
  price: number;
  percentSavings: number;
  priceBreaks: object;
  priceBreakRange: string[];
  selectedBreak: object;
  relatedProducts$: Observable<MarketplaceMeProduct[]>;
  imageUrls: string[] = [];
  favoriteProducts: string[] = [];
  qtyValid = true;
  supplierNote: string;
  specLength: number;
  quoteFormModal = ModalState.Closed;
  currentUser: User;
  showRequestSubmittedMessage = false;
  submittedQuoteOrder: any;
  constructor(
    private formService: SpecFormService,
    private context: ShopperContextService) {
    this.specFormService = formService;
  }

  @Input() set specs(value: ListSpec) {
    this._specs = value;
    this.specFormService.event.valid = this._specs.Items.length === 0;
    this.specLength = this._specs.Items.length;
  }

  @Input() set product(value: MarketplaceMeProduct) {
    this._product = value;
    this.isOrderable = !!this._product.PriceSchedule;
    this.imageUrls = this.getImageUrls();
    this.supplierNote = this._product.xp && this._product.xp.Note;
  }

  ngOnInit(): void {
    this.currentUser = this.context.currentUser.get();
    this.context.currentUser.onChange(user => (this.favoriteProducts = user.FavoriteProductIDs));
  }

  onSpecFormChange(event): void {
    if (event.detail.type === 'Change') {
      this.specFormService.event = event.detail;
      this.price = this.getTotalPrice();
    }
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

  getDocumentUrl(url: string) {
    let spliturl = url.split('/')
    spliturl.splice(4, 1);
    let str = spliturl.join('/')
    return str;
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

  openQuoteForm() {
    this.quoteFormModal = ModalState.Open;
  }

  isQuoteProduct(): boolean {
    return this._product.xp.ProductType === 'Quote';
  }

  dismissQuoteForm() {
    this.quoteFormModal = ModalState.Closed;
  }

  getDefaultQuoteOrder(user) {
    const defaultQuoteOrder = {
      xp: {
        BuyerLocationID: '',
        AvalaraTaxTransactionCode: '',
        OrderType: OrderType.Quote,
        QuoteOrderInfo: {
          FirstName: user.FirstName,
          LastName: user.LastName,
          Phone: user.Phone,
          Email: user.Email,
          Comments: user.Comments
        }
      }
    };
    return defaultQuoteOrder;
  }

  async submitQuoteOrder(user) {
    try {
      const defaultOrder = this.getDefaultQuoteOrder(user);
      const lineItem: LineItem = {};
      lineItem.ProductID = this._product.ID;
      lineItem.Product = this._product;
      lineItem.Specs = this.specFormService.getLineItemSpecs(this._specs);
      this.context.order.submitQuoteOrder(defaultOrder, lineItem).then(order => this.submittedQuoteOrder = order);
      this.quoteFormModal = ModalState.Closed;
      this.showRequestSubmittedMessage = true;
    } catch (ex) {
      this.showRequestSubmittedMessage = false;
      this.quoteFormModal = ModalState.Closed;
      throw ex;
    }
  }

  toOrderDetail() {
    this.context.router.toMyOrderDetails(this.submittedQuoteOrder.ID);
  }
}
