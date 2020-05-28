import { Component, Input, OnInit } from '@angular/core';
import { faTimes, faListUl, faTh } from '@fortawesome/free-solid-svg-icons';
import { ListSpec, User } from '@ordercloud/angular-sdk';
import { minBy as _minBy } from 'lodash';
import { LineItem, MarketplaceMeProduct, OrderType, ShopperContextService, MarketplaceLineItem, PriceSchedule } from 'marketplace';
import { Observable } from 'rxjs';
import { ModalState } from 'src/app/models/modal-state.class';
import { getImageUrls } from 'src/app/services/images.helpers';
import { SpecFormService } from '../spec-form/spec-form.service';
import { SuperMarketplaceProduct, ListPage, Asset } from '../../../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';
import { exchange } from 'src/app/services/currency.helper';
import { BuyerCurrency } from '../product-card/product-card.component';
import { ExchangeRates } from 'marketplace/projects/marketplace/src/lib/services/exchange-rates/exchange-rates.service';

@Component({
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class OCMProductDetails implements OnInit {
  faTh = faTh;
  faListUl = faListUl;
  faTimes = faTimes;
  _specs: ListSpec;
  _product: MarketplaceMeProduct;
  _priceSchedule: PriceSchedule;
  _rates: ListPage<ExchangeRates>;
  _orderCurrency: string;
  _attachments: Asset[] = [];
  specFormService: SpecFormService;
  isOrderable = false;
  quantity: number;
  _price: BuyerCurrency;
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
  showGrid = false;
  isAddingToCart = false;
  constructor(
    private formService: SpecFormService,
    private context: ShopperContextService) {
    this.specFormService = formService;
  }

  @Input() set product(superProduct: SuperMarketplaceProduct) {
    this._product = superProduct.Product;
    this._priceSchedule = superProduct.PriceSchedule;
    this._rates = this.context.exchangeRates.Get();
    this._attachments = superProduct?.Attachments; 
    const currentUser = this.context.currentUser.get();
    // Using `|| "USD"` for fallback right now in case there's bad data without the xp value.
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === "BuyerLocation")[0].xp?.Currency || "USD";
    this._price = exchange(this._rates, this.getTotalPrice(), this._product?.xp?.Currency, this._orderCurrency);
    // Specs
    this._specs = {Meta: {}, Items: superProduct.Specs};
    this.specFormService.event.valid = this._specs.Items.length === 0;
    this.specLength = this._specs.Items.length;
    // End Specs
    this.imageUrls = superProduct.Images.map(img => img.Url);
    this.isOrderable = !!superProduct.PriceSchedule;
    this.supplierNote = this._product.xp && this._product.xp.Note;
  }

  async ngOnInit(): Promise<void> {
    this.currentUser = this.context.currentUser.get();
    this.context.currentUser.onChange(user => (this.favoriteProducts = user.FavoriteProductIDs));
  }

  onSpecFormChange(event): void {
    if (event.detail.type === 'Change') {
      this.specFormService.event = event.detail;
      this._price = exchange(this._rates, this.getTotalPrice(), this._product?.xp?.Currency, this._orderCurrency);
    }
  }

  toggleGrid(bool) {
    this.showGrid = bool;
  }

  qtyChange(event: { qty: number; valid: boolean }): void {
    if (event.valid) {
      this.quantity = event.qty;
      this._price = exchange(this._rates, this.getTotalPrice(), this._product?.xp?.Currency, this._orderCurrency);
    }
  }

  async addToCart(): Promise<void> {
      this.isAddingToCart = true;
      try {
        await this.context.order.cart.add({
          ProductID: this._product.ID,
          Quantity: this.quantity,
          Specs: this.specFormService.getLineItemSpecs(this._specs),
        });
        this.isAddingToCart = false;
      } catch (ex) {
        this.isAddingToCart = false;
        throw ex;
      }
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
    if (!this._priceSchedule?.PriceBreaks.length) return;

    const priceBreaks = this._priceSchedule.PriceBreaks;
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

  async getImageUrls(): Promise<string[]> {
    return await getImageUrls(this._product)
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
      const lineItem: MarketplaceLineItem = {};
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
