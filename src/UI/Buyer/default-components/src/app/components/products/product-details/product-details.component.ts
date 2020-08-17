import { Component, Input, OnInit } from '@angular/core';
import { faTimes, faListUl, faTh } from '@fortawesome/free-solid-svg-icons';
import { Spec, PriceBreak } from 'ordercloud-javascript-sdk';
import { minBy as _minBy } from 'lodash';
import { MarketplaceMeProduct, ShopperContextService, CurrentUser } from 'marketplace';
import { PriceSchedule } from 'ordercloud-javascript-sdk';
import { MarketplaceLineItem, Asset, QuoteOrderInfo, LineItem } from '@ordercloud/headstart-sdk';
import { Observable } from 'rxjs';
import { ModalState } from 'src/app/models/modal-state.class';
import { SpecFormService } from '../spec-form/spec-form.service';
import { SuperMarketplaceProduct, ListPage } from '@ordercloud/headstart-sdk';
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
  _superProduct: SuperMarketplaceProduct;
  _specs: ListPage<Spec>;
  _product: MarketplaceMeProduct;
  _priceSchedule: PriceSchedule;
  _priceBreaks: PriceBreak[];
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
  images: Asset[] = [];
  imageUrls: string[] = [];
  favoriteProducts: string[] = [];
  qtyValid = true;
  supplierNote: string;
  _userCurrency: string;
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
    this._superProduct = superProduct;
    this._product = superProduct.Product;
    this._priceSchedule = superProduct.PriceSchedule as any;
    this._attachments = superProduct?.Attachments;
    const currentUser = this.context.currentUser.get();
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === 'BuyerLocation')[0]?.xp?.Currency;
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === 'BuyerLocation')[0].xp?.Currency;
    this._priceBreaks = superProduct.PriceSchedule?.PriceBreaks;
    this._price = this.getTotalPrice();
    // Specs
    this._specs = { Meta: {}, Items: superProduct.Specs as any };
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
    this._userCurrency = this.context.currentUser.get().Currency;
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
    this.qtyValid = event.valid;
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
            ImageUrl: this.specFormService.getLineItemImageUrl(this._superProduct)
          }
        });
        this.isAddingToCart = false;
      } catch (ex) {
        this.isAddingToCart = false;
        throw ex;
      }
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
      (((priceBreaks[0].Price - selectedBreak.Price) / priceBreaks[0].Price) * 100).toFixed(0), 10
    );
    return this.specFormService.event.valid
      ? this.specFormService.getSpecMarkup(this._specs, selectedBreak, this.quantity || startingBreak.Quantity)
      : selectedBreak.Price * (this.quantity || startingBreak.Quantity);
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

  async submitQuoteOrder(info: QuoteOrderInfo): Promise<void> {
    try {
      const lineItem: MarketplaceLineItem = {};
      lineItem.ProductID = this._product.ID;
      lineItem.Specs = this.specFormService.getLineItemSpecs(this._specs);
      lineItem.xp = {
        ImageUrl: this.specFormService.getLineItemImageUrl(this._product)
      };
      this.submittedQuoteOrder = await this.context.order.submitQuoteOrder(info, lineItem);
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
