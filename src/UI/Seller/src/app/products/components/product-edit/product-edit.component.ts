import { Component, Input, Output, EventEmitter, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { get as _get } from 'lodash';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { UserContext } from '@app-seller/config/user-context';
import {
  ListAddress,
  OcSupplierAddressService,
  OcAdminAddressService,
  OcProductService,
} from '@ordercloud/angular-sdk';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Product } from '@ordercloud/angular-sdk';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { faTrash, faTimes, faCircle, faHeart } from '@fortawesome/free-solid-svg-icons';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductService } from '@app-seller/products/product.service';
import { SuperMarketplaceProduct, ListPage, HeadStartSDK, SpecOption } from '@ordercloud/headstart-sdk';
import TaxCodes from 'marketplace-javascript-sdk/dist/api/TaxCodes';
import { Location } from '@angular/common'
import { TabIndexMapper, setProductEditTab } from './tab-mapper';
import { AppAuthService } from '@app-seller/auth';
import { environment } from 'src/environments/environment';
import { AssetUpload } from 'marketplace-javascript-sdk/dist/models/AssetUpload';
import { Asset } from 'marketplace-javascript-sdk/dist/models/Asset';
import { OcIntegrationsAPIService } from '@app-seller/shared/services/oc-integrations-api/oc-integrations-api.service';
import { SupportedRates } from '@app-seller/shared/models/supported-rates.interface';
import { SELLER } from '@app-seller/shared/models/ordercloud-user.types';
import { ValidateMinMax } from '../../../validators/validators';

@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.scss'],
})
export class ProductEditComponent implements OnInit {
  @Input()
  productForm: FormGroup;
  @Input()
  set orderCloudProduct(product: Product) {
    if (product.ID) {
      this.handleSelectedProductChange(product);
    } else {
      this.setTaxCodes(this.productService.emptyResource.Product.xp.Tax.Category)
      this.createProductForm(this.productService.emptyResource);
      this._superMarketplaceProductEditable = this.productService.emptyResource;
      this._superMarketplaceProductStatic = this.productService.emptyResource;
    }
  }
  @Input() readonly: boolean;
  @Input()
  filterConfig;
  @Output()
  updateResource = new EventEmitter<any>();
  @Input()
  addresses: ListAddress;
  @Input()
  isCreatingNew: boolean;
  @Input()
  dataIsSaving = false;
  userContext = {};
  hasVariations = false;
  images: Asset[] = [];
  files: FileHandle[] = [];
  faTimes = faTimes;
  faTrash = faTrash;
  faCircle = faCircle;
  faHeart = faHeart;
  _superMarketplaceProductStatic: SuperMarketplaceProduct;
  _superMarketplaceProductEditable: SuperMarketplaceProduct;
  supplierCurrency: SupportedRates;
  sellerCurrency: SupportedRates;
  _exchangeRates: SupportedRates[];
  areChanges = false;
  taxCodeCategorySelected = false;
  taxCodes: ListPage<TaxCodes>;
  productType: string;
  productVariations: any;
  variantsValid = true;
  editSpecs = false;
  fileType: string;
  imageFiles: FileHandle[] = [];
  staticContentFiles: FileHandle[] = [];
  staticContent: Asset[] = [];
  documentName: string;
  selectedTabIndex = 0;
  editPriceBreaks = false;
  newPriceBreakPrice = 0;
  newPriceBreakQty = 2;
  newProductPriceBreaks = [];
  availableProductTypes = [];
  active: number;
  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private router: Router,
    private location: Location,
    private currentUserService: CurrentUserService,
    private ocSupplierAddressService: OcSupplierAddressService,
    private ocProductService: OcProductService,
    private ocAdminAddressService: OcAdminAddressService,
    private productService: ProductService,
    private sanitizer: DomSanitizer,
    private modalService: NgbModal,
    private middleware: MiddlewareAPIService,
    private ocIntegrations: OcIntegrationsAPIService,
    private appAuthService: AppAuthService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) { }

  async ngOnInit(): Promise<void> {
    // TODO: Eventually move to a resolve so that they are there before the component instantiates.
    this.isCreatingNew = this.productService.checkIfCreatingNew();
    this.getAddresses();
    this.userContext = await this.currentUserService.getUserContext();
    if((this.userContext as any).UserType !== SELLER) {
      await this.getAvailableProductTypes();
    }
    this.setProductEditTab();
  }

  setProductEditTab(): void {
    const productDetailSection = this.router.url.split('/')[3];
    this.active = setProductEditTab(productDetailSection);
  }

  tabChanged(event: any, productID: string): void {
    const nextIndex = Number(event.nextId);
    if (productID === null) return;
    const newLocation = nextIndex === 0 ? `products/${productID}` : `products/${productID}/${TabIndexMapper[nextIndex]}`;
    this.location.replaceState(newLocation);
  }

  async getAddresses(): Promise<void> {
    const context: UserContext = await this.currentUserService.getUserContext();
    if (context.Me.Supplier) {
      this.addresses = await this.ocSupplierAddressService.List(context.Me.Supplier.ID).toPromise();
    } else {
      this.addresses = await this.ocAdminAddressService.List().toPromise();
    }
  }

  async setTaxCodes(taxCategory: string): Promise<any> {
    this.taxCodes = await this.listTaxCodes(taxCategory, '', 1, 100);
  }

  async refreshProductData(superProduct: SuperMarketplaceProduct): Promise<void> {
    // If a seller, and not editing the product, grab the currency from the product xp.
    this.supplierCurrency = this._exchangeRates?.find(r => r.Currency === superProduct?.Product?.xp?.Currency);
    // copying to break reference bugs
    this._superMarketplaceProductStatic = JSON.parse(JSON.stringify(superProduct));
    this._superMarketplaceProductEditable = JSON.parse(JSON.stringify(superProduct));
    if (!this._superMarketplaceProductEditable?.Product?.xp?.UnitOfMeasure) this._superMarketplaceProductEditable.Product.xp.UnitOfMeasure = { Unit: null, Qty: null };
    if (
      this._superMarketplaceProductEditable.Product?.xp?.Tax?.Category
    ) {
      await this.setTaxCodes(this._superMarketplaceProductEditable.Product.xp.Tax.Category)
    } else {
      this.taxCodes = { Meta: {}, Items: [] };
    }
    this.productType = this._superMarketplaceProductEditable.Product?.xp?.ProductType;
    this.staticContent = this._superMarketplaceProductEditable.Attachments;
    this.createProductForm(this._superMarketplaceProductEditable);
    this.images = this._superMarketplaceProductEditable.Images;
    this.taxCodeCategorySelected = this._superMarketplaceProductEditable.Product?.xp?.Tax?.Category !== null;
    this.isCreatingNew = this.productService.checkIfCreatingNew();
    this.checkForChanges();
  }

  createProductForm(superMarketplaceProduct: SuperMarketplaceProduct): void {
    if (superMarketplaceProduct.Product) {
      this.productForm = new FormGroup({
        Active: new FormControl(superMarketplaceProduct.Product.Active),
        Name: new FormControl(superMarketplaceProduct.Product.Name, [Validators.required, Validators.maxLength(100)]),
        ID: new FormControl(superMarketplaceProduct.Product.ID),
        Description: new FormControl(superMarketplaceProduct.Product.Description, Validators.maxLength(1000)),
        Inventory: new FormControl(superMarketplaceProduct.Product.Inventory),
        QuantityMultiplier: new FormControl(superMarketplaceProduct.Product.QuantityMultiplier),
        ShipFromAddressID: new FormControl(superMarketplaceProduct.Product.ShipFromAddressID),
        ShipHeight: new FormControl(superMarketplaceProduct.Product.ShipHeight, [Validators.required, Validators.min(0)]),
        ShipWidth: new FormControl(superMarketplaceProduct.Product.ShipWidth, [Validators.required, Validators.min(0)]),
        ShipLength: new FormControl(superMarketplaceProduct.Product.ShipLength, [Validators.required, Validators.min(0)]),
        ShipWeight: new FormControl(superMarketplaceProduct.Product.ShipWeight, [Validators.required, Validators.min(0)]),
        Price: new FormControl(_get(superMarketplaceProduct.PriceSchedule, 'PriceBreaks[0].Price', null)),
        MinQuantity: new FormControl(superMarketplaceProduct.PriceSchedule.MinQuantity, Validators.min(1)),
        MaxQuantity: new FormControl(superMarketplaceProduct.PriceSchedule.MaxQuantity, Validators.min(1)),
        UseCumulativeQuantity: new FormControl(superMarketplaceProduct.PriceSchedule.UseCumulativeQuantity),
        Note: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Note'), Validators.maxLength(140)),
        ProductType: new FormControl(_get(superMarketplaceProduct.Product, 'xp.ProductType'), Validators.required),
        IsResale: new FormControl(_get(superMarketplaceProduct.Product, 'xp.IsResale')),
        QuantityAvailable: new FormControl(superMarketplaceProduct.Product?.Inventory?.QuantityAvailable, null),
        InventoryEnabled: new FormControl(_get(superMarketplaceProduct.Product, 'Inventory.Enabled')),
        OrderCanExceed: new FormControl(_get(superMarketplaceProduct.Product, 'Inventory.OrderCanExceed')),
        TaxCodeCategory: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Tax.Category', null)),
        TaxCode: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Tax.Code', null)),
        UnitOfMeasureUnit: new FormControl(_get(superMarketplaceProduct.Product, 'xp.UnitOfMeasure.Unit'), Validators.required),
        UnitOfMeasureQty: new FormControl(_get(superMarketplaceProduct.Product, 'xp.UnitOfMeasure.Qty'), Validators.required),
      }, { validators: ValidateMinMax }
      );
      this.setInventoryValidator();
    }
  }

  setInventoryValidator() {
    const quantityControl = this.productForm.get('QuantityAvailable');
    this.productForm.get('InventoryEnabled').valueChanges
    .subscribe(inventory => {
      if(inventory) {
        quantityControl.setValidators([Validators.required, Validators.min(1)]);
      } else {
        quantityControl.setValidators(null); 
      }
      quantityControl.updateValueAndValidity()
    })
  }

  async getAvailableProductTypes(): Promise<void> {
    const supplier = await this.currentUserService.getMySupplier();
    this.availableProductTypes = supplier.xp.ProductTypes || []; 
  }
  
  async handleSave(): Promise<void> {
    if (this.isCreatingNew) {
      await this.createNewProduct();
    } else {
      this.updateProduct();
    }
  }

  async handleDelete(): Promise<void> {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    await HeadStartSDK.Products.Delete(this._superMarketplaceProductStatic.Product.ID, accessToken);
    this.router.navigateByUrl('/products');
  }

  handleDiscardChanges(): void {
    this.imageFiles = [];
    this.staticContentFiles = [];
    this._superMarketplaceProductEditable = this._superMarketplaceProductStatic;
    this.refreshProductData(this._superMarketplaceProductStatic);
  }

  async createNewProduct(): Promise<void> {
    try {
      this.dataIsSaving = true;
      const superProduct = await this.createNewSuperMarketplaceProduct(this._superMarketplaceProductEditable);
      if (this.imageFiles.length > 0) await this.addImages(this.imageFiles, superProduct.Product.ID);
      if (this.staticContentFiles.length > 0) await this.addDocuments(this.staticContentFiles, superProduct.Product.ID);
      this.refreshProductData(superProduct);
      this.router.navigateByUrl(`/products/${superProduct.Product.ID}`);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async updateProduct(): Promise<void> {
    try {
      this.dataIsSaving = true;
      let superProduct = this._superMarketplaceProductStatic;
      if (JSON.stringify(this._superMarketplaceProductEditable) !== JSON.stringify(this._superMarketplaceProductStatic)) {
        superProduct = await this.updateMarketplaceProduct(this._superMarketplaceProductEditable);
      }
      this.refreshProductData(superProduct);
      if (this.imageFiles.length > 0) await this.addImages(this.imageFiles, superProduct.Product.ID);
      if (this.staticContentFiles.length > 0) {
        await this.addDocuments(this.staticContentFiles, superProduct.Product.ID);
      }
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  updateProductResource(productUpdate: any): void {
    const resourceToUpdate = this._superMarketplaceProductEditable || this.productService.emptyResource;
    this._superMarketplaceProductEditable = this.productService.getUpdatedEditableResource(productUpdate, resourceToUpdate);
    this.checkForChanges();
  }

  handleUpdateProduct(event: any, field: string, typeOfValue?: string): void {
    const productUpdate = {
      field,
      value:
        ['Product.Active', 'Product.xp.IsResale', 'Product.Inventory.Enabled', 'Product.Inventory.OrderCanExceed'].includes(field)
          ? event.target.checked : typeOfValue === 'number' ? Number(event.target.value) : event.target.value
    };
    this.updateProductResource(productUpdate);
  }

  // Used only for Product.Description coming out of quill editor (no 'event.target'.)
  updateResourceFromFieldValue(field: string, value: any): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this._superMarketplaceProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Product = {
      ...updateProductResourceCopy.Product,
      [field]: value,
    };
    this._superMarketplaceProductEditable = updateProductResourceCopy;
    this.checkForChanges();
  }
  // TODO: Remove duplicate function, function exists in resource-crud.component.ts (minus the files check);
  checkForChanges(): void {
    this.areChanges =
      JSON.stringify(this._superMarketplaceProductEditable) !== JSON.stringify(this._superMarketplaceProductStatic) ||
      this.imageFiles?.length > 0 || this.staticContentFiles?.length > 0;
  }

  /** ****************************************
   *  **** PRODUCT IMAGE UPLOAD FUNCTIONS ****
   * ******************************************/

  manualFileUpload(event, fileType: string): void {
    if (fileType === 'image') {
      const files: FileHandle[] = Array.from(event.target.files).map((file: File) => {
        const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
        return { File: file, URL };
      });
      this.stageImages(files);
    } else if (fileType === 'staticContent') {

      const files: FileHandle[] = Array.from(event.target.files).map((file: File) => {
        const URL = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
        return { File: file, URL, Filename: this.documentName };
      });
      this.documentName = null;
      this.stageDocuments(files);
    }
  }

  stageImages(files: FileHandle[]): void {
    this.imageFiles = this.imageFiles.concat(files);
    this.checkForChanges();
  }

  async uploadAsset(productID: string, file: FileHandle, assetType: string): Promise<SuperMarketplaceProduct> {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const asset: AssetUpload = {
      Active: true,
      File: file.File,
      Type: (assetType as AssetUpload['Type']),
      FileName: file.Filename
    }
    const newAsset: Asset = await HeadStartSDK.Upload.UploadAsset(asset, accessToken);
    await HeadStartSDK.ProductContents.SaveAssetAssignment(productID, newAsset.ID, accessToken);
    return await HeadStartSDK.Products.Get(productID, accessToken);
  }

  async addDocuments(files: FileHandle[], productID: string): Promise<void> {
    let superProduct;
    for (const file of files) {
      superProduct = await this.uploadAsset(productID, file, 'Attachment');
    }
    this.staticContentFiles = [];
    // Only need the `|| {}` to account for creating new product where this._superMarketplaceProductStatic doesn't exist yet.
    superProduct = Object.assign(this._superMarketplaceProductStatic || {}, superProduct);
    this.refreshProductData(superProduct);
  }

  async addImages(files: FileHandle[], productID: string): Promise<void> {
    let superProduct;
    for (const file of files) {
      superProduct = await this.uploadAsset(productID, file, 'Image');
    }
    this.imageFiles = []
    // Only need the `|| {}` to account for creating new product where this._superMarketplaceProductStatic doesn't exist yet.
    superProduct = Object.assign(this._superMarketplaceProductStatic || {}, superProduct);
    this.refreshProductData(superProduct);
  }

  async removeFile(file: Asset): Promise<void> {
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    await HeadStartSDK.Assets.Delete(file.ID, accessToken);
    if (file.Type === 'Image') {
      this._superMarketplaceProductStatic.Images = this._superMarketplaceProductStatic.Images.filter(i => i.ID !== file.ID); 
    } else {
      this._superMarketplaceProductStatic.Attachments = this._superMarketplaceProductStatic.Attachments.filter(a => a.ID !== file.ID);
    }
    this.refreshProductData(this._superMarketplaceProductStatic);
  }

  unstageFile(index: number, fileType: string): void {
    if (fileType === 'image') {
      this.imageFiles.splice(index, 1)
    } else {
      this.staticContentFiles.splice(index, 1);
    }
    this.checkForChanges();
  }

  /** ****************************************
   *  *** PRODUCT DOCUMENT UPLOAD FUNCTIONS ***
   * ******************************************/

  /* This url points to the document in blob storage in order for it to be downloadable. */

  getDocumentName(event: KeyboardEvent): void {
    this.documentName = (event.target as HTMLInputElement).value;
  }

  stageDocuments(files: FileHandle[]): void {
    files.forEach(file => {
      const fileName = file.File.name.split('.');
      const ext = fileName[1];
      const fileNameWithExt = file.Filename + '.' + ext;
      file.Filename = fileNameWithExt;
    });
    this.staticContentFiles = this.staticContentFiles.concat(files);
    this.checkForChanges();
  }

  open(content): void {
    this.modalService.open(content, { ariaLabelledBy: 'confirm-modal' });
  }

  async handleTaxCodeCategorySelection(event): Promise<void> {
    // TODO: This is a temporary fix to accomodate for data not having xp.TaxCode yet
    if (
      this._superMarketplaceProductEditable?.Product?.xp &&
      !this._superMarketplaceProductEditable.Product.xp.Tax
    ) {
      this._superMarketplaceProductEditable.Product.xp.Tax = { Category: '', Code: '', Description: '' };
    }
    this.resetTaxCodeAndDescription();
    this.handleUpdateProduct(event, 'Product.xp.Tax.Category');
    this._superMarketplaceProductEditable.Product.xp.Tax.Code = '';
    await this.setTaxCodes(event.target.value)
  }
  // Reset TaxCode Code and Description if a new TaxCode Category is selected
  resetTaxCodeAndDescription(): void {
    this.handleUpdateProduct({ target: { value: null } }, 'Product.xp.Tax.Code');
    this.handleUpdateProduct({ target: { value: null } }, 'Product.xp.Tax.Description');
  }

  async searchTaxCodes(searchTerm: string): Promise<void> {
    if (searchTerm === undefined) searchTerm = '';
    const taxCodeCategory = this._superMarketplaceProductEditable.Product.xp.Tax.Category;
    await this.setTaxCodes(taxCodeCategory)
  }

  async handleScrollEnd(searchTerm: string): Promise<void> {
    if (searchTerm === undefined) searchTerm = '';
    const totalPages = this.taxCodes.Meta.TotalPages;
    const nextPageNumber = this.taxCodes.Meta.Page + 1;
    if (totalPages > nextPageNumber) {
      const taxCodeCategory = this._superMarketplaceProductEditable.Product.xp.Tax.Category;
      const avalaraTaxCodes = await this.listTaxCodes(taxCodeCategory, searchTerm, nextPageNumber, 100);
      this.taxCodes = {
        Meta: avalaraTaxCodes.Meta,
        Items: [...this.taxCodes.Items, ...avalaraTaxCodes.Items],
      };
      this.changeDetectorRef.detectChanges();
    }
  }

  getSaveBtnText(): string {
    return this.productService.getSaveBtnText(this.dataIsSaving, this.isCreatingNew)
  }

  async createNewSuperMarketplaceProduct(
    superMarketplaceProduct: SuperMarketplaceProduct
  ): Promise<SuperMarketplaceProduct> {
    const supplier = await this.currentUserService.getMySupplier();
    superMarketplaceProduct.Product.xp.Status = 'Draft';
    superMarketplaceProduct.Product.xp.Currency = supplier?.xp?.Currency;
    superMarketplaceProduct.PriceSchedule.ID = superMarketplaceProduct.Product.ID;
    superMarketplaceProduct.PriceSchedule.Name = `Default_Marketplace_Buyer${superMarketplaceProduct.Product.Name}`;
    return await HeadStartSDK.Products.Post(superMarketplaceProduct);
  }

  async updateMarketplaceProduct(superMarketplaceProduct: SuperMarketplaceProduct): Promise<SuperMarketplaceProduct> {
    // TODO: Temporary while Product set doesn't reflect the current strongly typed Xp
    superMarketplaceProduct.Product.xp.Status = 'Draft';
    return await HeadStartSDK.Products.Put(superMarketplaceProduct.Product.ID, superMarketplaceProduct);
  };

  async handleSelectedProductChange(product: Product): Promise<void> {
    this._exchangeRates = await this.ocIntegrations.getAvailableCurrencies();
    const currencyOnProduct = product.xp.Currency;
    this.supplierCurrency = this._exchangeRates?.find(r => r.Currency === currencyOnProduct);
    this.sellerCurrency = this._exchangeRates?.find(r => r.Currency === 'USD');
    const accessToken = await this.appAuthService.fetchToken().toPromise();
    const marketplaceProduct = await HeadStartSDK.Products.Get(product.ID, accessToken);
    this.refreshProductData(marketplaceProduct);
  }

  async listTaxCodes(taxCategory, search, page, pageSize): Promise<any> {
    return await HeadStartSDK.Avalaras.ListTaxCodes({ filters: { Category: taxCategory }, search, page, pageSize });
  }

  getTotalMarkup = (specOptions: SpecOption[]): number => {
    let totalMarkup = 0;
    if (specOptions) {
      specOptions.forEach(opt => opt.PriceMarkup ? totalMarkup = +totalMarkup + +opt.PriceMarkup : 0);
    }
    return totalMarkup;
  }

  updateEditableProductWithVariationChanges(e): void {
    const updateProductResourceCopy = this.productService.copyResource(
      this._superMarketplaceProductEditable || this.productService.emptyResource
    );
    updateProductResourceCopy.Specs = e.Specs;
    updateProductResourceCopy.Variants = e.Variants;
    this._superMarketplaceProductEditable = updateProductResourceCopy;
    this.checkForChanges();
  }

  validateVariants(e): void {
    this.variantsValid = e;
  }

  shouldIsResaleBeChecked(): boolean {
    return this._superMarketplaceProductEditable?.Product?.xp?.IsResale;
  }

  getProductPreviewImage(): string | SafeUrl {
    return this.imageFiles[0]?.URL || `${environment.middlewareUrl}/products/${this._superMarketplaceProductEditable?.Product?.ID}/image`;
  }
}
