import { Component, Input, OnInit } from '@angular/core';
import { faTimes, faListUl, faTh } from '@fortawesome/free-solid-svg-icons';
import { ListSpec } from '@ordercloud/angular-sdk';
import { minBy as _minBy } from 'lodash';
import { MarketplaceMeProduct, ShopperContextService, PriceSchedule, OrderType, ExchangeRates, CurrentUser } from 'marketplace';
import { MarketplaceLineItem, AssetForDelivery, MarketplaceOrder, QuoteOrderInfo } from 'marketplace-javascript-sdk';
import { Observable } from 'rxjs';
import { ModalState } from 'src/app/models/modal-state.class';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';
import { SpecFormService } from '../spec-form/spec-form.service';
import { SuperMarketplaceProduct, ListPage, Asset } from '../../../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';
import { exchange } from 'src/app/services/currency.helper';
import { BuyerCurrency, ExchangedPriceBreak } from 'src/app/models/currency.interface';
import { SpecFormEvent } from '../spec-form/spec-form-values.interface';
import { QtyChangeEvent } from '../quantity-input/quantity-input.component';

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
  _priceBreaks: ExchangedPriceBreak[];
  _rates: ListPage<ExchangeRates>;
  _orderCurrency: string;
  _attachments: Asset[] = [];
  specFormService: SpecFormService;
  isOrderable = false;
  quantity: number;
  _price: number;
  percentSavings: number;
  priceBreaks: object;
  priceBreakRange: string[];
  selectedBreak: object;
  relatedProducts$: Observable<MarketplaceMeProduct[]>;
  images: AssetForDelivery[] = [];
  imageUrls: string[] = [];
  favoriteProducts: string[] = [];
  qtyValid = true;
  supplierNote: string;
  specLength: number;
  quoteFormModal = ModalState.Closed;
  currentUser: CurrentUser;
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
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === 'BuyerLocation')[0].xp?.Currency;
    this._priceBreaks = superProduct.PriceSchedule?.PriceBreaks.map(pb => {
      const newPrice: BuyerCurrency = exchange(this._rates, pb.Price, superProduct.Product?.xp?.Currency, this._orderCurrency);
      const exchanged: ExchangedPriceBreak = {
        Quantity: pb.Quantity,
        Price: newPrice
      }
      return exchanged;
    });
    this._price = this.getTotalPrice();
    // Specs
    this._specs = { Meta: {}, Items: superProduct.Specs };
    this.specFormService.event.valid = this._specs.Items.length === 0;
    this.specLength = this._specs.Items.length;
    // End Specs
    this.images = superProduct.Images.map(img => img);
    this.imageUrls = superProduct.Images.map(img => img.Url);
    this.isOrderable = !!superProduct.PriceSchedule;
    this.supplierNote = this._product.xp && this._product.xp.Note;
  }

  ngOnInit(): void {
    this.currentUser = this.context.currentUser.get();
    this.context.currentUser.onChange(user => (this.favoriteProducts = user.FavoriteProductIDs));
  }

  onSpecFormChange(event: SpecFormEvent): void {
    if (event.type === 'Change') {
      this.specFormService.event = event;
      this._price = this.getTotalPrice();
    }
  }

  toggleGrid(showGrid: boolean): void {
    this.showGrid = showGrid;
  }

  qtyChange(event: QtyChangeEvent): void {
    if (event.valid) {
      this.quantity = event.qty;
      this._price = this.getTotalPrice();
    }
  }

  async addToCart(): Promise<void> {
      this.isAddingToCart = true;
      try {
        await this.context.order.cart.add({
          ProductID: this._product.ID,
          Quantity: this.quantity,
          Specs: this.specFormService.getLineItemSpecs(this._specs),
          xp: {
            LineItemImageUrl: this.getLineItemImageUrl(this._product)
          }
        });
        this.isAddingToCart = false;
      } catch (ex) {
        this.isAddingToCart = false;
        throw ex;
      }
  }

  getLineItemImageUrl(product: MarketplaceMeProduct): string {
    const image = this.images.find(img => this.isImageMatchingSpecs(img));
    return image ? image.Url : getPrimaryImageUrl(product);
  }

  isImageMatchingSpecs(image: AssetForDelivery): boolean {
    const specs = this.specFormService.getLineItemSpecs(this._specs);
    return specs.every(spec => image.Tags.find(tag => tag.split('-').includes(spec.Value)));
  }

  getPriceBreakRange(index: number): string {
    if (!this._priceSchedule?.PriceBreaks.length) return '';
    const priceBreaks = this._priceSchedule.PriceBreaks;
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
    if (!this._priceBreaks.length) return;

    const priceBreaks = this._priceBreaks;
    const startingBreak = _minBy(priceBreaks, 'Quantity');
    const selectedBreak = priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity && candidate.Quantity <= this.quantity ? candidate : current;
    }, startingBreak);
    this.selectedBreak = selectedBreak;
    this.percentSavings = parseInt(
      (((priceBreaks[0].Price.Price - selectedBreak.Price.Price) / priceBreaks[0].Price.Price) * 100).toFixed(0), 10
    );
    return this.specFormService.event.valid
      ? this.specFormService.getSpecMarkup(this._specs, selectedBreak, this.quantity || startingBreak.Quantity)
      : selectedBreak.Price.Price * (this.quantity || startingBreak.Quantity);
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

  openQuoteForm(): void {
    this.quoteFormModal = ModalState.Open;
  }

  isQuoteProduct(): boolean {
    return this._product.xp.ProductType === 'Quote';
  }

  dismissQuoteForm(): void {
    this.quoteFormModal = ModalState.Closed;
  }

  getDefaultQuoteOrder(user: QuoteOrderInfo): MarketplaceOrder {
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

  async submitQuoteOrder(user: QuoteOrderInfo): Promise<void> {
    try {
      const defaultOrder = this.getDefaultQuoteOrder(user);
      const lineItem: MarketplaceLineItem = {};
      lineItem.ProductID = this._product.ID;
      lineItem.Specs = this.specFormService.getLineItemSpecs(this._specs);
      this.submittedQuoteOrder = await this.context.order.submitQuoteOrder(defaultOrder, lineItem);
      this.quoteFormModal = ModalState.Closed;
      this.showRequestSubmittedMessage = true;
    } catch (ex) {
      this.showRequestSubmittedMessage = false;
      this.quoteFormModal = ModalState.Closed;
      throw ex;
    }
  }

  toOrderDetail(): void {
    this.context.router.toMyOrderDetails(this.submittedQuoteOrder.ID);
  }
}
