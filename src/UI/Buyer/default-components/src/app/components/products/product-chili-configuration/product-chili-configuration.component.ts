import { Component, Input, OnInit } from '@angular/core';
import { faTimes, faListUl, faTh } from '@fortawesome/free-solid-svg-icons';
import { Spec, PriceBreak, Product } from 'ordercloud-javascript-sdk';
import { minBy as _minBy } from 'lodash';
import { MarketplaceMeProduct, ShopperContextService, CurrentUser } from 'marketplace';
import { PriceSchedule } from 'ordercloud-javascript-sdk';
import { MarketplaceLineItem, Asset, QuoteOrderInfo, LineItem, MarketplaceKitProduct, ProductInKit, ChiliConfig, ChiliSpec, ChiliTemplate } from '@ordercloud/headstart-sdk';
import { Observable } from 'rxjs';
import { ModalState } from 'src/app/models/modal-state.class';
import { SpecFormService } from '../spec-form/spec-form.service';
import { SuperMarketplaceProduct, ListPage } from '@ordercloud/headstart-sdk';
import { SpecFormEvent } from '../spec-form/spec-form-values.interface';
import { QtyChangeEvent } from '../quantity-input/quantity-input.component';
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';

declare var SetVariableValue: any;
declare var saveDocument: any;
declare var LoadChiliEditor: any;

@Component({
  templateUrl: './product-chili-configuration.component.html',
  styleUrls: ['./product-chili-configuration.component.scss'],
})
export class OCMProductChiliConfig implements OnInit {
  faTh = faTh;
  faListUl = faListUl;
  faTimes = faTimes;
  _superProduct: SuperMarketplaceProduct;
  _specs: ListPage<Spec>;
  _product: MarketplaceMeProduct;
  _kitProduct: MarketplaceKitProduct;
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
  isKitProduct: boolean;
  productsIncludedInKit: ProductInKit[];
  ocProductsInKit: any[];
  isKitStatic = false;
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
    private formService: SpecFormService,
    private sanitizer: DomSanitizer,
    private context: ShopperContextService) {
    this.specFormService = formService;
  }

  @Input() set template(chiliTemplate: ChiliTemplate) {
    this.chiliTemplate = chiliTemplate;
    this.isKitProduct = false;
    this._superProduct = chiliTemplate.Product;
    this._product = chiliTemplate.Product.Product;
    this._priceSchedule = chiliTemplate.Product.PriceSchedule as any;
    this._attachments = chiliTemplate.Product?.Attachments;
    const currentUser = this.context.currentUser.get();
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === 'BuyerLocation')[0]?.xp?.Currency;
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === 'BuyerLocation')[0].xp?.Currency;
    this._priceBreaks = chiliTemplate.Product.PriceSchedule?.PriceBreaks;
    this._price = this.getTotalPrice();
    // Specs
    this._specs = { Meta: {}, Items: chiliTemplate.Product.Specs as any };
    this.specFormService.event.valid = this._specs.Items.length === 0;
    this.specLength = this._specs.Items.length;
    // End Specs
    this.images = chiliTemplate.Product.Images.map(img => img);
    this.imageUrls = chiliTemplate.Product.Images.map(img => img.Url);
    this.isOrderable = !!chiliTemplate.Product.PriceSchedule;
    this.supplierNote = this._product.xp && this._product.xp.Note;
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
    SetVariableValue((spec.ListOrder - 1), event.target, types[spec.xp.UI.ControlType]);
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
    setTimeout(async() => {
      await this.context.chiliConfig.getChiliProof(this.currentDocID, "4511001").then(proofURL => {
        this.lineImage = proofURL;
        this.context.chiliConfig.getChiliPDF(this.currentDocID, "4511001").then(printArtworkURL => {
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
    this.currentUser = this.context.currentUser.get();
    this._userCurrency = this.context.currentUser.get().Currency;
    //this.loadScript('https://chili.accuconnect.com/CHILI/scripts/index.js');
    //this.loadScript('https://www.acculync.com/four51/scripts/nd_451_enc.js');
    
    this.loadScript('https://www.acculync.com/four51/scripts/sd_451_enc.js');
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

  async addKitToCart(): Promise<void> {
    this.isAddingToCart = true;
    try {
      let lineItems = [];
      if (this.isKitStatic) {
        this.productsIncludedInKit.forEach(product => {
          let i = this.ocProductsInKit.findIndex(p => p.ID === product.ID);
          lineItems.push({
            ProductID: product.ID,
            Product: this.ocProductsInKit[i],
            Quantity: product.MinQty,
            Specs: this.specFormService.getLineItemSpecs(this._specs),
            xp: this.ocProductsInKit[i].xp
          })
        })
        await this.context.order.cart.addMany(lineItems);
      }
    } catch (ex) {
      this.isAddingToCart = false;
      throw ex;
    }
    this.isAddingToCart = false;
  }

  async addToCart(): Promise<void> {
    this.isAddingToCart = true;
    try {
      this.context.order.cart.add({
        ProductID: this._product.ID,
        Quantity: this.quantity,
        Specs: this.specFormService.getLineItemSpecs(this._specs),
        xp: {
          PrintArtworkURL: this.pdfSrc,
          ImageUrl: this.lineImage
        }
      });
      this.isAddingToCart = false;
    } catch (ex) {
      this.isAddingToCart = false;
      throw ex;
    }    
  }

  async asyncForEach(array, cb) {
    for (let i = 0; i < array.length; i++) {
      await cb(array[i], i, array);
    }
  }

  async getProductsInKit(productsInKit: ProductInKit[]) {
    let meProducts = [];
    await this.asyncForEach(productsInKit, async (product) => {
      let meProduct = await this.context.tempSdk.getMeProduct(product.ID);
      meProducts.push(meProduct.Product);
    })
    this.ocProductsInKit = meProducts;
  }

  changeQuantity(productID: string, event: QtyChangeEvent) {
    let indexOfProduct;
    for (let i = 0; i < this.ocProductsInKit.length; i++) {
      if (this.ocProductsInKit[i].ID === productID) {
        indexOfProduct = i;
      }
    }
    const item = {
      Quantity: event.qty,
      Product: this._product,
      ProductID: this._product.ID,
    };
    // const i = this.lineItems.findIndex(li => li.ProductID === item.ProductID);
    // if (i === -1) this.lineItems.push(item);
    // else this.lineItems[i] = item;
    this.getTotalPrice();
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
    if (!this._priceBreaks?.length) return;

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
        ImageUrl: this.specFormService.getLineItemImageUrl(this._product),
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
