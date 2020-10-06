import { Component, Input, OnInit } from '@angular/core';
import { faTimes, faListUl, faTh } from '@fortawesome/free-solid-svg-icons';
import { Spec, PriceBreak } from 'ordercloud-javascript-sdk';
import { MarketplaceMeProduct, ShopperContextService, CurrentUser, ContactSupplierBody } from 'marketplace';
import { PriceSchedule } from 'ordercloud-javascript-sdk';
import { MarketplaceLineItem, Asset, QuoteOrderInfo, LineItem, MarketplaceKitProduct, ProductInKit, MarketplaceVariant } from '@ordercloud/headstart-sdk';
import { MarketplaceLineItem, Asset, QuoteOrderInfo } from '@ordercloud/headstart-sdk';
import { Observable } from 'rxjs';
import { ModalState } from 'src/app/models/modal-state.class';
import { SpecFormService } from '../spec-form/spec-form.service';
import { SuperMarketplaceProduct } from '@ordercloud/headstart-sdk';
import { SpecFormEvent } from '../spec-form/spec-form-values.interface';
import { QtyChangeEvent } from '../quantity-input/quantity-input.component';
import { FormGroup } from '@angular/forms';
import { ProductDetailService } from './product-detail.service';

@Component({
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class OCMProductDetails implements OnInit {
  // font awesome icons
  faTh = faTh;
  faListUl = faListUl;
  faTimes = faTimes;

  _superProduct: SuperMarketplaceProduct;
  specs: Spec[];
  _product: MarketplaceMeProduct;
  priceSchedule: PriceSchedule;
  priceBreaks: PriceBreak[];
  attachments: Asset[] = [];
  isOrderable = false;
  quantity: number;
  price: number;
  percentSavings: number;
  relatedProducts$: Observable<MarketplaceMeProduct[]>;
  favoriteProducts: string[] = [];
  qtyValid = true;
  supplierNote: string;
  userCurrency: string;
  quoteFormModal = ModalState.Closed;
  contactSupplierFormModal = ModalState.Closed;
  currentUser: CurrentUser;
  showRequestSubmittedMessage = false;
  showContactSupplierFormSubmittedMessage = false;
  submittedQuoteOrder: any;
  showGrid = false;
  isAddingToCart = false;
  contactRequest: ContactSupplierBody;
  specForm: FormGroup;
  constructor(
    private specFormService: SpecFormService,
    private context: ShopperContextService,
    private productDetailService: ProductDetailService
  ) {
  }
  @Input() set product(superProduct: SuperMarketplaceProduct) {
    this._superProduct = superProduct;
    this._product = superProduct.Product;
    this.attachments = superProduct?.Attachments;
    this.priceBreaks = superProduct.PriceSchedule?.PriceBreaks;
    this.isOrderable = !!superProduct.PriceSchedule;
    this.supplierNote = this._product.xp && this._product.xp.Note;
    this.specs = superProduct.Specs;
      this.populateInactiveVariants(superProduct);
  }

  ngOnInit(): void {
    this.calculatePrice();
    this.currentUser = this.context.currentUser.get();
    this.userCurrency = this.context.currentUser.get().Currency;
    this.context.currentUser.onChange(user => (this.favoriteProducts = user.FavoriteProductIDs));
  }

  onSpecFormChange(event: SpecFormEvent): void {
    this.specForm = event.form;
    this.calculatePrice();
  }

  onSelectionInactive(event: any){
    console.log(event);
    this.isInactiveVariant = event;
  }

  populateInactiveVariants(superProduct: SuperMarketplaceProduct) {
    this._disabledVariants = [];
    superProduct.Variants?.forEach(variant => {
      if (!variant.Active) {
        this._disabledVariants.push(variant);
      }
    })
  }

  toggleGrid(showGrid: boolean): void {
    this.showGrid = showGrid;
  }

  qtyChange(event: QtyChangeEvent): void {
    this.qtyValid = event.valid;
    if (event.valid) {
      this.quantity = event.qty;
      this.calculatePrice();
    }
  }

  calculatePrice(): void {
    this.price = this.productDetailService.getProductPrice(this.priceBreaks, this.specs, this.specForm, this.quantity);
    if (this.priceBreaks?.length) {
      const basePrice = this.quantity * this.priceBreaks[0].Price;
      this.percentSavings = this.productDetailService.getPercentSavings(this.price, basePrice)
    }
  }

  async addToCart(): Promise<void> {
    this.isAddingToCart = true;
    try {
      await this.context.order.cart.add({
        ProductID: this._product.ID,
        Quantity: this.quantity,
        Specs: this.specFormService.getLineItemSpecs(this.specs, this.specForm),
        xp: {
          ImageUrl: this.specFormService.getLineItemImageUrl(this._superProduct, this.specForm)
        }
      });
    } finally {
      this.isAddingToCart = false;
    }
  }

  getPriceBreakRange(index: number): string {
    if (!this.priceBreaks.length) return '';
    const indexOfNextPriceBreak = index + 1;
    if (indexOfNextPriceBreak < this.priceBreaks.length) {
      return `${this.priceBreaks[index].Quantity} - ${this.priceBreaks[indexOfNextPriceBreak].Quantity - 1}`;
    } else {
      return `${this.priceBreaks[index].Quantity}+`;
    }
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

  openContactSupplierForm(): void {
    this.contactSupplierFormModal = ModalState.Open;
  }

  isQuoteProduct(): boolean {
    return this._product.xp.ProductType === 'Quote';
  }

  dismissQuoteForm(): void {
    this.quoteFormModal = ModalState.Closed;
  }

  dismissContactSupplierForm(): void {
    this.contactSupplierFormModal = ModalState.Closed;
  }

  async submitQuoteOrder(info: QuoteOrderInfo): Promise<void> {
    try {
      const lineItem: MarketplaceLineItem = {};
      lineItem.ProductID = this._product.ID;
      lineItem.Specs = this.specFormService.getLineItemSpecs(this.specs, this.specForm);
      lineItem.xp = {
        ImageUrl: this.specFormService.getLineItemImageUrl(this._product, this.specForm),
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

  async submitContactSupplierForm(formData: any): Promise<void> {
    this.contactRequest = { Product: this._product, BuyerRequest: formData }
    try {
      await this.context.currentUser.submitContactSupplierForm(this.contactRequest);
      this.contactSupplierFormModal = ModalState.Closed;
      this.showContactSupplierFormSubmittedMessage = true;
    } catch (ex) {
      this.contactSupplierFormModal = ModalState.Closed;
      throw ex;
    }
  }

  toOrderDetail(): void {
    this.context.router.toMyOrderDetails(this.submittedQuoteOrder.ID);
  }
}
