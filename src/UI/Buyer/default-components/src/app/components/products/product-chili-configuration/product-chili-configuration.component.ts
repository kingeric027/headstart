import { Component, Input, OnInit } from '@angular/core';
import { faTimes, faListUl, faTh } from '@fortawesome/free-solid-svg-icons';
import { Spec, PriceBreak, SpecOption } from 'ordercloud-javascript-sdk';
import { MarketplaceMeProduct, ShopperContextService, CurrentUser, ContactSupplierBody } from 'marketplace';
import { PriceSchedule } from 'ordercloud-javascript-sdk';
import { MarketplaceLineItem, Asset, QuoteOrderInfo, LineItem, MarketplaceKitProduct, ProductInKit, MarketplaceVariant, ChiliSpec, ChiliConfig, ChiliTemplate } from '@ordercloud/headstart-sdk';
import { Observable } from 'rxjs';
import { ModalState } from 'src/app/models/modal-state.class';
import { SpecFormService } from '../spec-form/spec-form.service';
import { SuperMarketplaceProduct } from '@ordercloud/headstart-sdk';
import { SpecFormEvent } from '../spec-form/spec-form-values.interface';
import { QtyChangeEvent } from '../quantity-input/quantity-input.component';
import { FormGroup } from '@angular/forms';
import { ProductDetailService } from '../product-details/product-detail.service';
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';

declare var SetVariableValue: any;
declare var saveDocument: any;
declare var LoadChiliEditor: any;

@Component({
  templateUrl: './product-chili-configuration.component.html',
  styleUrls: ['./product-chili-configuration.component.scss'],
})
export class OCMProductChiliConfig implements OnInit {
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
  isKitProduct: boolean;
  productsIncludedInKit: ProductInKit[];
  ocProductsInKit: any[];
  isKitStatic = false;
  _chiliConfigs: ChiliConfig[] = [];
  showConfigs = false;
  contactRequest: ContactSupplierBody;
  specForm: FormGroup;
  isInactiveVariant: boolean;
  _disabledVariants: MarketplaceVariant[];
  variantInventory: number;
  chiliTemplate: ChiliTemplate;
  showSpecs = false;
  frameSrc: SafeResourceUrl;
  lineImage: string = '';
  pdfSrc: string = '';
  currentDocID = '';
  ShowAddToCart = false;
  isLoading = true;
  editor;
  frameWindow;

  constructor(
    private specFormService: SpecFormService,
    private sanitizer: DomSanitizer,
    private context: ShopperContextService,
    private productDetailService: ProductDetailService
  ) { }

  @Input() set template(chiliTemplate: ChiliTemplate) {
    this.chiliTemplate = chiliTemplate;
    this._superProduct = chiliTemplate.Product;
    this._product = chiliTemplate.Product.Product;
    this.attachments = chiliTemplate.Product?.Attachments;
    this.priceBreaks = chiliTemplate.Product.PriceSchedule?.PriceBreaks;
    this.isOrderable = !!chiliTemplate.Product.PriceSchedule;
    this.supplierNote = this._product.xp && this._product.xp.Note;
    this.specs = chiliTemplate.Product.Specs;
    this.populateInactiveVariants(chiliTemplate.Product);
    this.frameSrc = this.sanitizer.bypassSecurityTrustResourceUrl(this.chiliTemplate.Frame);
    this.currentDocID = this.frameSrc.toString().split("doc=")[1].split("&")[0];
    this.loadChiliEditor(this.chiliTemplate.Frame);
  }
  setTecraSpec(event: any, spec: ChiliSpec): void {
    const types = {
      'Text': 'string',
      'DropDown': 'list',
      'Checkbox': 'checkbox'
    };
    SetVariableValue((spec.ListOrder), event.target, types[spec.xp.UI.ControlType]);
  }

  loadChiliEditor(url: string): void {
    setTimeout(() => {
      LoadChiliEditor(url);
      this.isLoading = false;
    }, 1000);
  }

  async saveTecraDocument(): Promise<void> {
    this.isLoading = true;
    saveDocument();
    setTimeout(async () => {
      await this.context.chiliConfig.getChiliProof(this.currentDocID).then(proofURL => {
        this.lineImage = proofURL;
        this.context.chiliConfig.getChiliPDF(this.currentDocID).then(printArtworkURL => {
          this.pdfSrc = printArtworkURL;
          this.isLoading = false;
          this.ShowAddToCart = true;
        });
      });
    }, 5000);
  }

  loadScript(url: string) {
    let body = <HTMLDivElement>document.body;
    let script = document.createElement('script');
    script.innerHTML = '';
    script.src = url;
    script.async = true;
    script.defer = true;
    body.appendChild(script);
  }

  async ngOnInit(): Promise<void> {
    this.calculatePrice();
    this.currentUser = this.context.currentUser.get();
    this.userCurrency = this.currentUser.Currency;
    this.context.currentUser.onChange(user => (this.favoriteProducts = user.FavoriteProductIDs));
    this.loadScript('https://www.acculync.com/four51/scripts/sd_451_enc.js');
  }

  onSpecFormChange(event: SpecFormEvent): void {
    this.specForm = event.form;
    if (this._superProduct?.Product?.Inventory?.Enabled && this._superProduct?.Product?.Inventory?.VariantLevelTracking) {
      this.variantInventory = this.getVariantInventory();
    }
    this.calculatePrice();
  }

  getVariantInventory(): number {
    let specCombo = "";
    let specOptions: SpecOption[] = [];
    this._superProduct?.Specs?.forEach(s => s.Options.forEach(o => specOptions = specOptions.concat(o)));
    for (var i = 0; i < this.specForm.value.ctrls.length; i++) {
      const matchingOption = specOptions.find(o => o.Value === this.specForm.value.ctrls[i])
      i === 0 ? specCombo += matchingOption.ID : specCombo += `-${matchingOption.ID}`
    }
    return this._superProduct.Variants.find(v => v.xp?.SpecCombo === specCombo)?.Inventory?.QuantityAvailable
  }

  onSelectionInactive(event: boolean) {
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
          PrintArtworkURL: this.pdfSrc,
          ImageUrl: this.lineImage
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
        ImageUrl: this.specFormService.getLineItemImageUrl(this._superProduct.Images, this._superProduct.Specs, this.specForm),
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
